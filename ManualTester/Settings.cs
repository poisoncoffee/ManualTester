using System;
using System.IO;
using System.Collections.Generic;


namespace WindowsFormsApp1
{
    public static class Settings
    {
        static string executedDirectoryPath;

        public static string appsPackageName { get; private set; }

        public static string appsDefinitionsContainerPath { get; private set; }
        public static string sequenceDefinitionsFilePath { get; private set; }
        public static string stepDefinitionsFilePath { get; private set; }
        public static string customScriptsContainerPath { get; private set; }

        public static string allDefinitionsContainerPath { get; private set; }
        public static string logcatContainerDirectory { get; private set; }
        public static string briefLogcatFilePath { get; private set; }
        public static string detailedLogcatFilePath { get; private set; }

        public static void InitializeSettings()
        {
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            logcatContainerDirectory = executedDirectoryPath + @"\logcat\";
            allDefinitionsContainerPath = executedDirectoryPath + @"\apps\";
            briefLogcatFilePath = logcatContainerDirectory + @"\briefLogcat.txt";
            detailedLogcatFilePath = logcatContainerDirectory + @"\detailedLogcat.txt";
            GetAvailableAppsList();
        }

        public static void SelectApp(string packageName)
        {
            appsPackageName = packageName;
            SetPathsForCurrentlySelectedApp();
        }

        public static void SetPathsForCurrentlySelectedApp()
        {
            appsDefinitionsContainerPath = allDefinitionsContainerPath + appsPackageName + @"\";
            sequenceDefinitionsFilePath = allDefinitionsContainerPath + appsPackageName + @"\TestSequenceDefinitions.json";
            stepDefinitionsFilePath = allDefinitionsContainerPath + appsPackageName + @"\TestStepDefinitions.json";
            customScriptsContainerPath = allDefinitionsContainerPath + appsPackageName + @"\";
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

        public static List<string> GetAvailableAppsList()
        {
            string[] Subdirectories = Directory.GetDirectories(allDefinitionsContainerPath);
            List<string> AppsList = new List<string>();
            foreach(string path in Subdirectories)
            {
                AppsList.Add(ExtractDirectoryNameFromPath(path));
            } 
            return AppsList;
        }

        #region Helpers
        public static string ExtractDirectoryNameFromPath(string path)
        {
            string directoryName = "";
            for (int i = 0; i < path.Length; i++)
            {
                if (path[i] == '\\')
                    directoryName = "";
                else
                    directoryName += path[i];
            }

            return directoryName;
        }

        #endregion

    }
}
