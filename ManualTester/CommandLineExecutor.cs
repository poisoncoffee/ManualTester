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

        //TODO: Refactor (get rid of pseudoTimer)
        public static string ExecuteCommandGetOutputFromFile(string command, string fileName)
        {
            string filePath = "/" + fileName + ".txt";
            string filePathCopy = filePath + ".copy.txt";

            DeleteFileIfExists(filePath);
            DeleteFileIfExists(filePathCopy);

            command += " > " + filePath;
            ExecuteCommand(command);

            int pseudoTimer = 20;
            while (pseudoTimer > 0 && !File.Exists(filePath))
            {
                // Does nothing. This loop waits for the file to start existing which does not happen immediately.
                Thread.Sleep(500);
                pseudoTimer--;
            }

            Thread.Sleep(1000);

            if (!File.Exists(filePath))
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

        public static void ExecuteBat(string batName, string arguments)
        {
            Process cmdDetailed = new Process();
            cmdDetailed.StartInfo.UseShellExecute = true;
            cmdDetailed.StartInfo.FileName = batName + ".bat";
            if (arguments.Length > 0)
                cmdDetailed.StartInfo.Arguments = arguments;
            cmdDetailed.Start();
        }

        #region Helpers

        private static void DeleteFileIfExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        #endregion
    }
}
