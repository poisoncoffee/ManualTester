namespace WindowsFormsApp1
{
    class ADBWrapperSettings
    {
        public static string appLaunchBatFileName = "launchApp";
        public static string getDevicesResolutionFileName = "getDevicesResolution";

        public static string startDetailedLogcatFileName = "startDetailedLogcat";
        public static string startBriefLogcatFileName = "startBriefLogcat";


        public static string GetFileNameForResult(string batName)
        {
            return batName + ".result";
        }
    }
}
