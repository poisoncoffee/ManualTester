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
            briefLogcatSourcePath = executedDirectoryPath + "/briefLogcat.txt";         //is hardcoded in startBriefLogcat.bat file too
            detailedLogcatSourcePath = executedDirectoryPath + "/detailedLogcat.txt";   //is hardcoded in startDetailedLogcat.bat file too
        }

        public struct Logcat
        {
            public int briefLogcatProcessID { get; set; }
            public int detailedLogcatProcessID { get; set; }
            public string briefLogcatPath { get; set; }
            public string detailedLogcatPath { get; set; }
            public List<string> logs { get; set; }
        }

        #region adbMethods
        //runs cmd executing provided command and returns cmd output
        //not working well
        private static StreamReader ExecuteCommandGetOutput(string command)
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

        private static string ExecuteCommandToFile(string command, string fileName)
        {

            string filePath = executedDirectoryPath + "/" + fileName + ".txt";
            string filePathCopy = filePath + ".copy.txt";

            //deletes old files if they exist
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            if (File.Exists(filePathCopy))
            {
                File.Delete(filePathCopy);
            }


            //starting cmd process
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            //generating adb devices output file
            command += " > " + filePath;
            cmd.StandardInput.WriteLine(command);
            cmd.Close();


            while(!File.Exists(filePath))
            {
                // Does nothing. This loop waits for the file to start existing which does not happen immediately.
                Thread.Sleep(100);
            }

            File.Copy(filePath, filePathCopy, true);

            while (!File.Exists(filePathCopy))
            {
                // Does nothing. This loop waits for the file to start existing which does not happen immediately.
                Thread.Sleep(100);
            }

            //FileStream file = new FileStream(filePathCopy, FileMode.Open, FileAccess.ReadWrite);
            StreamReader file = new StreamReader(filePathCopy);
            string output = file.ReadToEnd();
            file.Close();        
            return output;
        }

        //if device has correct status and is ready for testing
        public static bool IsDeviceReady()
        { 

            StreamReader sr = ExecuteCommandGetOutput("adb devices");

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

        //returns ID of device if it's ready; else returns descriptive error message
        public static string GetDeviceStatus()
        {

            StreamReader sr = ExecuteCommandGetOutput("adb devices");

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

        public static bool LaunchApp(string packagename)
        {
            TestDatabase database = TestDatabase.Instance;
            string command = "adb shell am start -n " + packagename + "/" + database.GetMainActivityName(packagename);
            string launchAppResult = ExecuteCommandToFile(command, "launchAppResult");

            if (launchAppResult.Contains("Error") || launchAppResult.Contains("error") || launchAppResult.Contains("Exception") || launchAppResult.Contains("exception"))
                return false;

            return true;


        }

        //not really working because adb shell cmd package list packages returns output that never ends
        public static bool IsAppInstalled(string packagename)
        {
            string command = "adb shell cmd package list packages";
            StreamReader sr = ExecuteCommandGetOutput(command);

            string tempLine = "";
            while (sr.Peek() != -1)
                {
                tempLine = sr.ReadLine();
                if (tempLine.Contains(packagename))
                    return true;
                }

            return false;
        }

        //gets this Android process's ID
        public static string GetProcessPID(string packagename)
        {
            string processID = "";
            int pseudoTimer = 30;

            while (pseudoTimer > 0)
            {
                string command = "adb shell pidof -s " + packagename;
                processID = ExecuteCommandToFile(command, "GetProcessPID");

                if (processID != "")
                    {
                    processID = RemoveNonNumericFromString(processID);
                    return processID;
                    }

                //launching the process may take time so if no pid was found it retries 20 times and sleeps for 500 ms which gives 10 seconds for an app to launch.
                Thread.Sleep(500);
                pseudoTimer--;
            }

            throw new ArgumentException("Timeout: Couldn't get process's PID: " + processID + " is not valid or null. App is not installed or couldn't be launched.");

        }

        //inputs tap on the screen at the given coordinates
        public static void InputTap(int x, int y)
        {
            string command = "adb shell input tap " + x + " " + y;

            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            //getting output
            cmd.StandardInput.WriteLine(command);
            cmd.Close();
        }

        public static void InputBack()
        {
            ExecuteCommand("adb shell input keyevent 4");
        }
        #endregion

        #region LogcatLogic

        //begin logcat starts two processes collecting two logcats. One of them is detailed which will be helpful for debugging and the second one is brief which is useful for this program.
        //both of them need to be maintained
        public static Logcat BeginLogcat(string processPID, string packagename)
        {
            //forst, clean up the old files
            string briefLogcatPath = executedDirectoryPath + "/brieflLogcat.txt";
            string detailedLogcatPath = executedDirectoryPath + "/detailedlogcat.txt";
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

            //clears current logcat
            ExecuteCommand("adb logcat -c");

            //There is a reason .bat is used here. Cmd.exe crashes and stops logcat after a few minutes.
            //Runs new process for detailed logcat
            Process cmdDetailed = new Process();
            cmdDetailed.StartInfo.UseShellExecute = true;
            cmdDetailed.StartInfo.FileName = "startDetailedLogcat.bat";
            cmdDetailed.StartInfo.Arguments = processPID;
            cmdDetailed.Start();
            logcat.detailedLogcatProcessID = cmdDetailed.Id;
            logcat.detailedLogcatPath = executedDirectoryPath + "/detailedLogcat_" + packagename + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";

            //runs new process for brief logcat
            Process cmdBrief = new Process();
            cmdBrief.StartInfo.UseShellExecute = true;
            cmdBrief.StartInfo.FileName = "startBriefLogcat.bat";            
            cmdBrief.Start();
            logcat.briefLogcatProcessID = cmdBrief.Id;
            logcat.briefLogcatPath = executedDirectoryPath + "/briefLogcat_" + packagename + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";

            return logcat;
        }

        //todo
        public static void EndLogcat (Logcat logcat)
        {
            Process cmdBrief = Process.GetProcessById(logcat.briefLogcatProcessID);
            cmdBrief.Kill();
            Process cmdDetailed = Process.GetProcessById(logcat.detailedLogcatProcessID);
            cmdDetailed.Kill();
        }

        //refreshes logcat starting 
        public static Logcat UpdateLogcat(Logcat logcat, int offset)
        {

            //make copy from the "source" logcat (for both logcats)
            File.Copy(briefLogcatSourcePath, logcat.briefLogcatPath, true);
            File.Copy(detailedLogcatSourcePath, logcat.detailedLogcatPath, true);

            StreamReader file = new StreamReader(logcat.briefLogcatPath);
            string readLine = "";
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

        public static string RemoveWhiteSpacesFromString(string input)
        {
            //first, replaces all kind of possible whitespaces to ' ' <-- this space
            for (int i = 0; i < input.Length; i++)
            {
                if (char.IsWhiteSpace(input[i]))
                {
                    input.Replace(input[i], ' ');
                }
            }

            //second, replaces this particular space whith empty
            input.Replace(" ", string.Empty);
            return input;
        }

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
