using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

        public bool LaunchApp(Device device)
        {
            string arguments = device.serial + " " + Settings.chosenApp;
            string launchAppResult = CommandLineExecutor.ExecuteScriptGetOutput(ADBScriptedFunctionsSettings.GetFunctionsFilePath(
                ADBScriptedFunctionsSettings.EScriptedFunction.getResolution), 
                arguments, 
                ADBScriptedFunctionsSettings.GetFilePathForOutput(ADBScriptedFunctionsSettings.EScriptedFunction.appLaunch, device)
                ).ToLower() ;

            if (launchAppResult.Contains("error") || launchAppResult.Contains("exception"))
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

            string arguments = device.serial + " " + ADBScriptedFunctionsSettings.GetFilePathForOutput(ADBScriptedFunctionsSettings.EScriptedFunction.getResolution, device);
            string output = CommandLineExecutor.ExecuteScriptGetOutput(ADBScriptedFunctionsSettings.GetFunctionsFilePath(ADBScriptedFunctionsSettings.EScriptedFunction.getResolution), arguments, ADBScriptedFunctionsSettings.GetFilePathForOutput(ADBScriptedFunctionsSettings.EScriptedFunction.getResolution, device));
            MatchCollection Matches = Regex.Matches(output, @"(\d+)");
            if (Matches.Count == 2)      // Correct case - two values was found (Height and Width)
            {
                device.resolutionY = Int32.Parse(Matches[0].Value);
                device.resolutionX = Int32.Parse(Matches[1].Value);
                return device;
            }
            else                        // Incorrect case - output is different than expected
            {
                throw new ArgumentException("Could not get resolution for device " + device.serial + ". ADB output is: " + output);
            }
        }

        public bool IsAppInstalled() //TODO - should take Device in constructor and check it for specific device
        {
            string command = "adb shell cmd package list packages";
            StreamReader sr = CommandLineExecutor.ExecuteCommandGetOutput(command);

            string tempLine = "";
            while (sr.Peek() != -1)
            {
                tempLine = sr.ReadLine();
                if (tempLine.Contains(Settings.chosenApp))
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

        public void ExecuteInShell(string shellCommand)
        {
            string command = "adb shell " + shellCommand;
            CommandLineExecutor.ExecuteCommand(command);
        }

        public void ExecuteScript(string scriptName)
        {
            string scriptPath = Settings.appsDefinitionsContainerPath + scriptName;
            CommandLineExecutor.ExecuteScript(scriptName, string.Empty, Settings.appsDefinitionsContainerPath);
        }

        #endregion

        #region LogcatOperator

        //Begin logcat starts two processes collecting two logcats. Both of them need to be maintained.
        //One of them is "detailedLogcat" which is collecting all info related to this PID and it will be helpful for debugging
        //The second one is "briefLogcat" which is used by this program to determine conditions and actions of TestSteps.
        //The logic is:
        // 1. Check if files exist - if yes, delete them (they may be the result of the previous run)
        // 2. Clear device's logcat
        // 3. Run new process for both logcats 
        // 4. Set the path to which logcats will be copied and read
        // 5. Return logcat
        //There is a reason .bat is used here. Cmd.exe crashes and stops logging after a few minutes for unknown reasons.
        public Logcat BeginLogcat(string packagename)
        {
            //first, clean up the old files
            Helpers.DeleteFileIfExists(Settings.briefLogcatFilePath);
            Helpers.DeleteFileIfExists(Settings.detailedLogcatFilePath);

            //second, create new ones
            Logcat logcat = new Logcat();
            logcat.logs = new List<string>();
            Directory.CreateDirectory(Settings.logcatContainerDirectory);

            //clear device's logs
            CommandLineExecutor.ExecuteCommand("adb logcat -c");

            //run .bats that will start logcat
            CommandLineExecutor.ExecuteScript(ADBScriptedFunctionsSettings.GetFunctionsFilePath(ADBScriptedFunctionsSettings.EScriptedFunction.startBriefLogcat), String.Empty, String.Empty);
            CommandLineExecutor.ExecuteScript(ADBScriptedFunctionsSettings.GetFunctionsFilePath(ADBScriptedFunctionsSettings.EScriptedFunction.startDetailedLogcat), String.Empty, String.Empty);

            logcat.detailedLogcatPath = Settings.GetPathForCopy(Settings.briefLogcatFilePath);
            logcat.briefLogcatPath = Settings.GetPathForCopy(Settings.detailedLogcatFilePath);

            return logcat;
        }

        //The logic is:
        //1. For both logcats: Make copy from the "source" logcat.
        //2. For detailedLogcat: nothing else needs to be done. This program does not use it (it's kept here only for end-users: they may need full logs if the test fails).
        //3. For briefLogcat: Just ReadLine(); through logs until it reaches offset
        //3. Read logs after the offset and place them into Logcat struct
        //4. Return Logcat
        public Logcat UpdateLogcat(Logcat logcat, int offset)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("Trying to update Logcat with incorrect offset: " + offset);

            File.Copy(Settings.briefLogcatFilePath, logcat.briefLogcatPath, true);
            File.Copy(Settings.detailedLogcatFilePath, logcat.detailedLogcatPath, true);

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
            throw new NotImplementedException("Ending Logcat is not implemented yet.");
        }
        #endregion

    }
}
