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

        public static void ExecuteScript(string scriptName, string arguments, string workingDirectory)
        {
            Process cmd = new Process();
            cmd.StartInfo.UseShellExecute = true;
            cmd.StartInfo.FileName = scriptName;
            if (workingDirectory.Length > 0)
                cmd.StartInfo.WorkingDirectory = workingDirectory;
            if (arguments.Length > 0)
                cmd.StartInfo.Arguments = arguments;
            cmd.Start();
        }

        public static string ExecuteScriptGetOutput(string scriptPath, string arguments, string outputPath)
        {
            string outputCopyPath = Settings.GetPathForCopy(outputPath);
            string output = "" ;
            Helpers.DeleteFileIfExists(outputPath);
            Helpers.DeleteFileIfExists(outputCopyPath);
            ExecuteScript(scriptPath, arguments, string.Empty);
            SpinWait.SpinUntil(() => File.Exists(outputPath), 7000);

            if (!File.Exists(outputPath))
            {
                throw new IOException("File: " + outputPath + " that should be created by script " + scriptPath + "does not exist");
            }

            while (output == "")
            {
                File.Copy(outputPath, outputCopyPath, true);
                StreamReader file = new StreamReader(outputCopyPath);
                output = file.ReadToEnd();
                file.Close();
            }

            return output;
        }

    }
}
