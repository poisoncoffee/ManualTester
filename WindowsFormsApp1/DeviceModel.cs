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
        static DeviceModel()
        {

        }

        //runs cmd executing provided command and returns cmd output
        private static StreamReader ExecuteCommand(string command)
        {
            //starting cmd process
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            //getting adb devices output
            cmd.StandardInput.WriteLine(command);
            StreamReader output = cmd.StandardOutput;
            cmd.StandardInput.WriteLine("exit");
            return output;
        }
 
        //if device has correct status and is ready for testing
        public static bool IsDeviceReady()
        { 

            StreamReader sr = ExecuteCommand("adb devices");

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

            StreamReader sr = ExecuteCommand("adb devices");

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
            StreamReader sr = ExecuteCommand("adb shell am start -n " + packagename + "/" + database.GetMainActivityName(packagename) );

            string tempLine = "";
            while (sr.Peek() != -1)
            {
                tempLine = sr.ReadLine();
                if (tempLine.Contains("Error") || tempLine.Contains("error") || tempLine.Contains("Exception") || tempLine.Contains("exception"))
                    return false;
            }

            return true;


        }

        public static bool IsAppInstalled(string packagename)
        {
            StreamReader sr = ExecuteCommand("adb shell pm list packages -3");

            string tempLine = "";
            while (sr.Peek() != -1)
                {
                tempLine = sr.ReadLine();
                if (tempLine.Contains(packagename))
                    return true;
                }

            return false;

        }
    }
}
