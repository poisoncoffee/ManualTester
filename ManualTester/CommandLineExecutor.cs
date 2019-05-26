using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WindowsFormsApp1
{
    public class CommandLineExecutor
    {
        public static Process CreateProcess()
        {
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.CreateNoWindow = true;
            return cmd;
        }

        public static void ExecuteCommand(string command)
        {
            Process cmd = CreateProcess();
            cmd.Start();
            cmd.StandardInput.WriteLine(command);
            cmd.Close();
        }

        public static StreamReader ExecuteCommandGetOutput(string command)
        {
            Process cmd = CreateProcess();
            cmd.Start();
            cmd.StandardInput.WriteLine(command);
            StreamReader output = cmd.StandardOutput;
            cmd.StandardInput.WriteLine("exit");
            cmd.WaitForExit();
            return output;
        }

        public static void ExecuteBat(string batName, string arguments, string workingDirectory)
        {
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = true;
            cmd.StartInfo.FileName = batName;
            if (workingDirectory.Length > 0)
                cmd.StartInfo.WorkingDirectory = workingDirectory;
            if (arguments.Length > 0)
                cmd.StartInfo.Arguments = arguments;
            cmd.Start();
        }

        public static string ExecuteBatGetOutput(string batName, string arguments, string outputFileName)
        {
            string filePath = Settings.GetExecutedDirectoryPath() + @"\" + outputFileName;
            string filePathCopy = Settings.GetPathForCopy(filePath);
            string output = "" ;
            Helpers.DeleteFileIfExists(filePath);
            Helpers.DeleteFileIfExists(filePathCopy);
            ExecuteBat(batName, arguments, string.Empty);
            SpinWait.SpinUntil(() => File.Exists(filePath), 7000);

            if (!File.Exists(filePath))
            {
                throw new IOException("File: " + filePath + " that should be created by bat " + batName + "does not exist");
            }

            while (output == "")
            {
                File.Copy(filePath, filePathCopy, true);
                StreamReader file = new StreamReader(filePathCopy);
                output = file.ReadToEnd();
                file.Close();
            }

            return output;
        }

    }
}
