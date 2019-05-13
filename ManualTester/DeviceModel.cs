using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class DeviceModel
    {
        private static string executedDirectoryPath = "";
        private static string briefLogcatSourcePath = "";
        private static string detailedLogcatSourcePath = "";


        static DeviceModel()
        {
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            briefLogcatSourcePath = executedDirectoryPath + "/logcat/briefLogcat.txt";         //needs to be defined in startBriefLogcat.bat file too
            detailedLogcatSourcePath = executedDirectoryPath + "/logcat/detailedLogcat.txt";   //needs to be defined in startDetailedLogcat.bat file too
        }

        #region ExecutingCommands

        public static void ExecuteCommand(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            cmd.StandardInput.WriteLine(command);
            cmd.Close();
        }

        private static StreamReader GetExecutedCommandOutput(string command)
        {
            //starting cmd process
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            //getting output
            cmd.StandardInput.WriteLine(command);
            StreamReader output = cmd.StandardOutput;
            cmd.StandardInput.WriteLine("exit");
            cmd.WaitForExit();
            return output;
        }

        //High concept of this logic is:
        //1. Delete old files (probably from the previous run) if they exist
        //2. Start cmd process
        //3. Do empty loops until the file starts existing which does not happen immediately and there is no control of it
        //4. If the file exist
        private static string ExecuteCommandToFile(string command, string fileName)
        {
            string filePath = executedDirectoryPath + "/" + fileName + ".txt";
            string filePathCopy = filePath + ".copy.txt";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(filePathCopy))
            {
                File.Delete(filePathCopy);
            }

            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();
            command += " > " + filePath;
            cmd.StandardInput.WriteLine(command);
            cmd.Close();

            int pseudoTimer = 20;
            while(pseudoTimer > 0 && !File.Exists(filePath))
            {
                // Does nothing. This loop waits for the file to start existing which does not happen immediately.
                Thread.Sleep(500);
                pseudoTimer--;
            }

            if(!File.Exists(filePath))
            {
                throw new IOException("File: " + filePath + " that should be created by command " + command + "does not exist");
            }

            else
            {
                File.Copy(filePath, filePathCopy, true);

                StreamReader file = new StreamReader(filePathCopy);
                string output = file.ReadToEnd();
                file.Close();
                return output;
            }
        }

        // This method returns .bat output as string
        // There is a .bat requirement: .bat file, needs to redirect input to .txt file named exactly the same as the bat
        // Please provide batName without file extension.
        // High concept of this ugly logic is:
        // 1. Check if file exists - if yes, delete it (it may be the result of the previous run)
        // 2. Run .bat
        // 3. Wait for .bat output to start existing, which does not happen immediately (wait until pseudoTimer runs out). If timer runs out (approx. 10 seconds), throw an exception (assuming that the file won't be created)
        // 4. When file begins existing try to open it (and retry if the operation failed - the file may still be "in progress of creation" and copying it will fail). That happens until pseudoTimer2 runs out. If timer runs out (approx. 10 seconds), throw an exception (assuming that the file couldn't be open)
        private static string ExecuteBatToFile(string batName, string arguments)
        {            
            string outputFilePath = executedDirectoryPath + "/" + batName + ".txt";
            string output = "";
            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            Process cmdDetailed = new Process();
            cmdDetailed.StartInfo.UseShellExecute = true;
            cmdDetailed.StartInfo.FileName = batName + ".bat";
            if (arguments.Length > 0)
                cmdDetailed.StartInfo.Arguments = arguments;
            cmdDetailed.Start();

            int pseudoTimer = 20;
            while(pseudoTimer > 0 && !File.Exists(outputFilePath))
            {
                // do nothing - wait for file to start existing (which will not happen immediately)
                Thread.Sleep(100);
                pseudoTimer--;
            }

            // now, the timer ran out or the file started existing, so...
            if(!File.Exists(outputFilePath))
            {
                throw new IOException("File containig output: " + outputFilePath + " that should be created by bat file: " + batName + ".bat does not exist");
            } 
            else
            {
                string outputCopyPath = outputFilePath + ".copy.txt";
                int pseudoTimer2 = 20;

                while (pseudoTimer2 > 0)
                {
                    //this is all trycatched because the program often it tries to copy file while it's still used by the .bat file (.bat is still creating this file).
                    try
                    {
                        output = File.ReadAllText(outputFilePath);
                        return output;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                        pseudoTimer2--;
                    }
                }

                throw new IOException("Unable to open file: " + outputFilePath + " that should be created by bat file: " + batName);
            }
        }

        #endregion

        #region ExecutingActions

        //Launches an app with provided packagename. Returns true if launch succeeded and false if launch failed.
        public static bool LaunchApp(string packagename)
        {
            TestDatabase database = TestDatabase.Instance;
            string command = "adb shell am start -n " + packagename + "/" + database.GetMainActivityName(packagename);
            string launchAppResult = ExecuteCommandToFile(command, "launchAppResult");

            if (launchAppResult.Contains("Error") || launchAppResult.Contains("error") || launchAppResult.Contains("Exception") || launchAppResult.Contains("exception"))
                return false;
            else
                return true;
        }

        public static void InputTap(int x, int y)
        {
            string command = "adb shell input tap " + x + " " + y;
            ExecuteCommand(command);
        }

        public static void InputBack()
        {
            ExecuteCommand("adb shell input keyevent 4");
        }

        #endregion

        #region GetDeviceInfos

        public static bool IsDeviceReady()
        { 
            StreamReader sr = GetExecutedCommandOutput("adb devices");

            int devicesCount = 0;
            string tempLine = "";

            //searching for  device statuses ("device", "offline", "unauthorized") reading output line by line
            while (sr.Peek() != -1)
            {
                tempLine = sr.ReadLine();
                //if there is a device connected with correct status
                if (tempLine.Contains("device") && !tempLine.Contains("devices"))
                {
                    devicesCount++;
                }
                // if there is a device but with incorrect status
                else if (tempLine.Contains("offline") || tempLine.Contains("unauthorized"))
                {
                    return false;
                }
            }

            // only one device connected and with correct status
            if (devicesCount == 1)
            {
                return true;
            }
            // any other case
            else
            {
                return false;
            }
        }

        //Returns ID of device if the device is ready; otherwise returns descriptive error message
        public static string GetDeviceStatus()
        {

            StreamReader sr = GetExecutedCommandOutput("adb devices");

            int devicesCount = 0;
            string tempLine = "";
            string status = "";

            //searching for  device statuses ("device", "offline", "unauthorized") reading output line by line
            while (sr.Peek() != -1)
            {
                tempLine = sr.ReadLine();
                //if there is a device connected with correct status
                if (tempLine.Contains("device") && !tempLine.Contains("devices"))
                {
                    status = tempLine.Remove(tempLine.Length - 7);
                    devicesCount++;
                }
                // if there is a device but with incorrect status
                else if (tempLine.Contains("offline") || tempLine.Contains("unauthorized"))
                {
                    status = "Device offline or unauthorized";
                    devicesCount++;
                }
            }

            // only one device connected
            if (devicesCount == 1)
            {
                return status;
            }
            // more than one device connected
            else if (devicesCount > 1)
            {
                return "More than one device connected";
            }
            // no devices
            else
            {
                return "No devices found";
            }

        }

        //Not used, on To Fix List. While loop never ends as if there was no EOF.
        public static bool IsAppInstalled(string packagename)
        {
            string command = "adb shell cmd package list packages";
            StreamReader sr = GetExecutedCommandOutput(command);

            string tempLine = "";
            while (sr.Peek() != -1)
                {
                tempLine = sr.ReadLine();
                if (tempLine.Contains(packagename))
                    return true;
                }

            return false;
        }

        public static string GetProcessPID(string packagename)
        {
            string processID = "";

            //Launching an app on external device may take some time and there is no control of it. If no PID was found this method sleeps for 500 ms, and then retries - total of 20 (pseudoTimer) times, which gives 10 seconds for an app to launch. After that time it assumes that launch failed or something went wrong and an exception will be thrown.
            int pseudoTimer = 20;

            while (pseudoTimer > 0)
            {
                processID = ExecuteBatToFile("getPID", packagename);
                if (processID != "")
                {
                    processID = RemoveNonNumericFromString(processID);
                    return processID;
                }
                Thread.Sleep(500);
                pseudoTimer--;
            }

            throw new ArgumentException("Timeout: Couldn't get process's PID: " + processID + " is not valid or null. App is not installed or couldn't be launched.");
        }

        #endregion

        #region LogcatLogic

        public struct Logcat
        {
            public string briefLogcatPath { get; set; }
            public string detailedLogcatPath { get; set; }
            public List<string> logs { get; set; }
        }

        //Begin logcat starts two processes collecting two logcats. Both of them need to be maintained.
        //There is a reason .bat is used here. Cmd.exe crashes and stops logging after a few minutes.
        //One of them is "detailedLogcat" which is collecting all info related to this PID and it will be helpful for debugging
        //The second one is "briefLogcat" which is used by this program to determine conditions and actions of TestSteps.
        //The logic is:
        // 1. Check if files exist - if yes, delete them (they may be the result of the previous run)
        // 2. Clear device's logcat
        // 3. Run new process for detailed logcat
        // 4. Run new process for brief logcat
        // 5. Return logcat struct
        public static Logcat BeginLogcat(string processPID, string packagename)
        {
            //first, clean up the old files
            string briefLogcatPath = executedDirectoryPath + "/logcat/brieflLogcat.txt";
            string detailedLogcatPath = executedDirectoryPath + "/logcat/detailedlogcat.txt";

            if(File.Exists(briefLogcatPath))
            {
                File.Delete(briefLogcatPath);
            }
            if (File.Exists(detailedLogcatPath))
            {
                File.Delete(detailedLogcatPath);
            }

            Logcat logcat = new Logcat();
            logcat.logs = new List<string>();

            ExecuteCommand("adb logcat -c");

            Process cmdDetailed = new Process();
            cmdDetailed.StartInfo.UseShellExecute = true;
            cmdDetailed.StartInfo.FileName = "startDetailedLogcat.bat";
            cmdDetailed.StartInfo.Arguments = processPID;
            cmdDetailed.Start();
            logcat.detailedLogcatPath = executedDirectoryPath + "/logcat/detailedLogcat_" + packagename + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";
            cmdDetailed.Close();

            Process cmdBrief = new Process();
            cmdBrief.StartInfo.UseShellExecute = true;
            cmdBrief.StartInfo.FileName = "startBriefLogcat.bat";            
            cmdBrief.Start();
            logcat.briefLogcatPath = executedDirectoryPath + "/logcat/briefLogcat_" + packagename + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";
            cmdBrief.Close();

            return logcat;
        }

        //TODO
        public static void EndLogcat (Logcat logcat)
        {

        }

        //The logic is:
        //1. Make copy from the "source" logcat (for both logcats)
        //2. Just ReadLine(); through logs  until it reaches offset
        //3. Read logs after the offset and place them into Logcat struct
        //4. Return Logcat
        public static Logcat UpdateLogcat(Logcat logcat, int offset)
        {
            File.Copy(briefLogcatSourcePath, logcat.briefLogcatPath, true);
            File.Copy(detailedLogcatSourcePath, logcat.detailedLogcatPath, true);

            StreamReader file = new StreamReader(logcat.briefLogcatPath);
            int linesToSkip = offset;
            while (linesToSkip > 0)
            {
                file.ReadLine();
                linesToSkip--;
            }

            while(!file.EndOfStream)
            {
                while(logcat.logs.Count <= offset)
                {
                    //if there are less elements than "offset" (which is the index we will be inserting a value at), add an element.
                    logcat.logs.Add("");
                }
                logcat.logs[offset] = file.ReadLine();
                offset++;
            }

            file.Close();
            return logcat;
        }

        #endregion

        #region Helpers

        public static string RemoveNonNumericFromString(string input)
        {
            //first, replaces all kind of possible whitespaces to 'a'
            string output = "";
            foreach (char c in input)
            {
                if (char.IsNumber(c))
                {
                    output += c;
                }
            }
            return output;
        }

        #endregion
    }
}
