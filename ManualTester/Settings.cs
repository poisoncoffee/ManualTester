using System;


namespace WindowsFormsApp1
{
    public static class Settings
    {
        static string executedDirectoryPath;

        public static string appsPackageName { get; private set; }

        public static string appsDefinitionsDirectoryPath { get; private set; }
        public static string sequenceDefinitionsFilePath { get; private set; }
        public static string stepDefinitionsFilePath { get; private set; }
        public static string customScriptsContainerPath { get; private set; }

        public static string logcatContainerDirectory { get; private set; }
        public static string briefLogcatFilePath { get; private set; }
        public static string detailedLogcatFilePath { get; private set; }

        public static void InitializeSettings()
        {
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            logcatContainerDirectory = executedDirectoryPath + @"\logcat\";
        }

        public static void SelectApp(string packageName)
        {
            appsPackageName = packageName;
            SetPathsForCurrentlySelectedApp();
        }

        public static void SetPathsForCurrentlySelectedApp()
        {
            appsDefinitionsDirectoryPath = executedDirectoryPath + @"\apps\" + appsPackageName + @"\";
            sequenceDefinitionsFilePath = executedDirectoryPath + @"\apps\" + appsPackageName + @"\TestSequenceDefinitions.json";
            stepDefinitionsFilePath = executedDirectoryPath + @"\apps\" + appsPackageName + @"\TestStepDefinitions.json";
            customScriptsContainerPath = executedDirectoryPath + @"\apps\" + appsPackageName + @"\";

            logcatContainerDirectory = executedDirectoryPath + @"\logcat\";
            briefLogcatFilePath = logcatContainerDirectory + @"\briefLogcat.txt";
            detailedLogcatFilePath = logcatContainerDirectory + @"\detailedLogcat.txt";
        }

        public static string GetExecutedDirectoryPath()
        {
            return executedDirectoryPath;
        }

        public static string GetPathForCopy(string sourcePath)
        {
            string path = sourcePath + "." + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".txt";
            return path;
        }

    }
}
