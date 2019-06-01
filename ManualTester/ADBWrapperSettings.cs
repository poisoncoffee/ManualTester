using System.Collections.Generic;
using System;


namespace WindowsFormsApp1
{
    class ADBWrapperSettings
    {
        public enum EBatFunction
        {
            appLaunch,
            getResolution,
            startDetailedLogcat,
            startBriefLogcat
        }

        public static string batFilesDirectory = Settings.GetExecutedDirectoryPath() + @"\bats\";

        public static Dictionary<EBatFunction, string> FilePathsForBatFunctions = new Dictionary<EBatFunction, string>
        {
            { EBatFunction.appLaunch, batFilesDirectory + "appLaunch.bat" },
            { EBatFunction.getResolution, batFilesDirectory + "getDevicesResolution.bat" },
            { EBatFunction.startDetailedLogcat, batFilesDirectory + "startDetailedLogcat.bat" },
            { EBatFunction.startBriefLogcat, batFilesDirectory + "startBriefLogcat.bat" }
        };

        public static string GetFunctionsFilePath(EBatFunction Function)
        {
            foreach(KeyValuePair<EBatFunction, string> pair in FilePathsForBatFunctions)
            {
                if (pair.Key == Function)
                    return pair.Value;
            }

            throw new ArgumentException("There is no file associated with function " + Enum.GetName(typeof(EBatFunction), Function));
        }

        public static string GetFilePathForOutput(EBatFunction Function, Device device)
        {
            return batFilesDirectory + device.serial + "_" + Enum.GetName(typeof(EBatFunction), Function) + ".txt";
        }

    }
}
