using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace WindowsFormsApp1
{
    class ADBWrapper : IDeviceModel, ILogcatOperator
    {

        #region InputsAndActions
        public void InputBack()
        {
            CommandLineExecutor.ExecuteCommand("adb shell input keyevent 4");
        }

        public void InputTap(int x, int y)
        {
            string command = "adb shell input tap " + x + " " + y;
            CommandLineExecutor.ExecuteCommand(command);
        }

        public bool LaunchApp(string packageName)
        {
            TestDatabase database = TestDatabase.Instance;
            string command = "adb shell am start -n " + packageName + "/" + database.GetMainActivityName(packageName);
            string launchAppResult = CommandLineExecutor.ExecuteCommandGetOutputFromFile(command, "launchAppResult");

            if (launchAppResult.Contains("Error") || launchAppResult.Contains("error") || launchAppResult.Contains("Exception") || launchAppResult.Contains("exception"))
                return false;
            else
                return true;
        }

        #endregion

        #region ObtainingDeviceStatus

        public List<Device> GetConnectedDevices()
        {
            List<Device> ConnectedDevices = new List<Device>();
            StreamReader sr = CommandLineExecutor.ExecuteCommandGetOutput("adb devices");
            string tempLine = "";
            while (sr.Peek() != -1)
            {
                tempLine = sr.ReadLine();
                //if there is a device connected with correct status
                if (tempLine.Contains("device") && !tempLine.Contains("devices"))
                {
                    Device device = new Device();
                    device.status = Device.Status.Ready;
                    device.serial = tempLine.Remove(tempLine.Length - 7);
                    ConnectedDevices.Add(device);
                }
                // if there is a device but with incorrect status
                else if (tempLine.Contains("offline"))
                {
                    Device device = new Device();
                    device.status = Device.Status.Offline;
                    device.serial = tempLine.Remove(tempLine.Length - 8);
                    ConnectedDevices.Add(device);
                }
                else if (tempLine.Contains("unauthorized"))
                {
                    Device device = new Device();
                    device.status = Device.Status.Unauthorized;
                    device.serial = tempLine.Remove(tempLine.Length - 13);
                    ConnectedDevices.Add(device);
                }
            }

            return ConnectedDevices;
        }

        public List<Device> GetReadyDevicesFullInfo()
        {
            List<Device> ConnectedDevices = GetConnectedDevices();
            List<Device> ReadyDevices = new List<Device>();

            foreach (Device device in ConnectedDevices)
            {
                if (device.status == Device.Status.Ready)
                {
                    GetDevicesResolution(device);
                    ReadyDevices.Add(device);
                }
            }

            return ReadyDevices;
        }

        public Device GetDevicesResolution(Device device)
        {
            string command = "adb -s " + device.serial + " shell wm size";

            string output = CommandLineExecutor.ExecuteCommandGetOutputFromFile(command, "resolution");
            MatchCollection Matches = Regex.Matches(output, @"(\d+)");
            if (Matches.Count == 2)      // Correct case - two values was found (Height and Width)
            {
                device.resolutionY = Int32.Parse(Matches[0].Value);
                device.resolutionX = Int32.Parse(Matches[1].Value);
                return device;
            }
            else                        // Incorrect case - something went wrong (values are incorrect)
            {
                throw new ArgumentException("Could not get resolution for device " + device.serial + ". ADB output is: " + output);
            }
        }

        public bool IsAppInstalled(string packageName)
        {
            string command = "adb shell cmd package list packages";
            StreamReader sr = CommandLineExecutor.ExecuteCommandGetOutput(command);

            string tempLine = "";
            while (sr.Peek() != -1)
            {
                tempLine = sr.ReadLine();
                if (tempLine.Contains(packageName))
                    return true;
            }

            return false;
        }

        //For now, only one device can be connected, and this device's status must be ready. If this two requirements are met, device is ready.
        public bool IsDeviceReady()
        {
            List<Device> ConnectedDevices = GetConnectedDevices();
            if (ConnectedDevices.Count == 1 && ConnectedDevices[0].status == Device.Status.Ready)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region LogcatOperator

        //Begin logcat starts two processes collecting two logcats. Both of them need to be maintained.
        //One of them is "detailedLogcat" which is collecting all info related to this PID and it will be helpful for debugging
        //The second one is "briefLogcat" which is used by this program to determine conditions and actions of TestSteps.
        //The logic is:
        // 1. Check if files exist - if yes, delete them (they may be the result of the previous run)
        // 2. Clear device's logcat
        // 3. Run new process for detailed logcat
        // 4. Run new process for brief logcat
        // 5. Return logcat
        //There is a reason .bat is used here. Cmd.exe crashes and stops logging after a few minutes.
        public Logcat BeginLogcat(string packagename)
        {
            //first, clean up the old files
            string briefLogcatPath = "/logcat/brieflLogcat.txt";
            string detailedLogcatPath = "/logcat/detailedlogcat.txt";

            if (File.Exists(briefLogcatPath))
            {
                File.Delete(briefLogcatPath);
            }
            if (File.Exists(detailedLogcatPath))
            {
                File.Delete(detailedLogcatPath);
            }

            //second, create new ones
            Logcat logcat = new Logcat();
            logcat.logs = new List<string>();
            string directoryPath = "/logcat/";
            Directory.CreateDirectory(directoryPath);

            //clear device's logs
            CommandLineExecutor.ExecuteCommand("adb logcat -c");

            CommandLineExecutor.ExecuteBat("startDetailedLogcat.bat", string.Empty);
            CommandLineExecutor.ExecuteBat("startBriefLogcat.bat", String.Empty);
            logcat.detailedLogcatPath = Settings.GetExecutedDirectoryPath() + "/logcat/detailedLogcat_" + packagename + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";
            logcat.briefLogcatPath = Settings.GetExecutedDirectoryPath() + "/logcat/briefLogcat_" + packagename + "_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";

            return logcat;
        }

        //The logic is:
        //1. Make copy from the "source" logcat (for both logcats)
        //2. Just ReadLine(); through logs  until it reaches offset
        //3. Read logs after the offset and place them into Logcat struct
        //4. Return Logcat
        public Logcat UpdateLogcat(Logcat logcat, int offset)
        {
            string sourceBriefLogcatPath = Settings.GetExecutedDirectoryPath() + @"/logcat/briefLogcat.txt";
            string sourceDetailedLogcatPath = Settings.GetExecutedDirectoryPath() + @"/logcat/detailedLogcat.txt";
            File.Copy(sourceBriefLogcatPath, logcat.briefLogcatPath, true);
            File.Copy(sourceDetailedLogcatPath, logcat.detailedLogcatPath, true);

            StreamReader file = new StreamReader(logcat.briefLogcatPath);
            int linesToSkip = offset;
            while (linesToSkip > 0)
            {
                file.ReadLine();
                linesToSkip--;
            }

            while (!file.EndOfStream)
            {
                while (logcat.logs.Count <= offset)
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

        //TODO
        public void EndLogcat(Logcat logcat)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Helpers

        public static string RemoveNonNumericFromString(string input)
        {
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


        public static void WaitUntil(Func<bool> condition, int frequency, int maxTime)
        {
            while (!condition() && maxTime > 0)
            {
                Thread.Sleep(frequency);
                maxTime -= frequency;
            }

            if (condition())
                return;
            else
                throw new TimeoutException();
        }

        public readonly System.Threading.EventWaitHandle waitHandle = new System.Threading.AutoResetEvent(false);

        public void SetWaitHandleIf(Func<bool> condition)
        {
            while(!condition())
            {
                //Nie robi nic ew. Thread.Sleep(time);
            }

            waitHandle.Set();
        }

        public void WaitForSignalOrTimeout(int seconds)
        {
            waitHandle.WaitOne(seconds);
        }
        #endregion
    }
}
