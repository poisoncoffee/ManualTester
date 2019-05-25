namespace WindowsFormsApp1
{
    public static class Settings
    {
        static string executedDirectoryPath;

        public static string appsPackageName { get; private set; }
        public static string sequenceDefinitionsFilePath { get; private set; }
        public static string stepDefinitionsFilePath { get; private set; }
        public static string customScriptsContainerPath { get; private set; }

        public static string logcatContainerDirectory { get; private set; }

        public static void InitializeSettings()
        {
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            logcatContainerDirectory = executedDirectoryPath + @"\logcat\";
        }

        public static void SelectApp(string packageName)
        {
            appsPackageName = packageName;
            SetPathsFor(packageName);
        }

        public static void SetPathsFor(string packageName)
        {
            sequenceDefinitionsFilePath = executedDirectoryPath + @"\apps\" + packageName + @"\TestSequenceDefinitions.json";
            stepDefinitionsFilePath = executedDirectoryPath + @"\apps\" + packageName + @"\TestStepDefinitions.json";
            customScriptsContainerPath = executedDirectoryPath + @"\apps\" + packageName + @"\";
        }

        public static string GetExecutedDirectoryPath()
        {
            return executedDirectoryPath;
        }

    }
}
