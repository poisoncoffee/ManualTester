using System.Collections.Generic;
using System;

namespace WindowsFormsApp1
{
    class ADBScriptedFunctionsSettings
    {

        public static string scriptFilesDirectory = Settings.GetExecutedDirectoryPath() + @"\bats\";

        public enum EScriptedFunction
        {
            appLaunch,
            getResolution,
            startDetailedLogcat,
            startBriefLogcat
        }

        private static Dictionary<EScriptedFunction, string> ScriptPathsForFunctions = new Dictionary<EScriptedFunction, string>
        {
            { EScriptedFunction.appLaunch, scriptFilesDirectory + "appLaunch.bat" },
            { EScriptedFunction.getResolution, scriptFilesDirectory + "getDevicesResolution.bat" },
            { EScriptedFunction.startDetailedLogcat, scriptFilesDirectory + "startDetailedLogcat.bat" },
            { EScriptedFunction.startBriefLogcat, scriptFilesDirectory + "startBriefLogcat.bat" }
        };

        public static string GetFunctionsFilePath(EScriptedFunction Function)
        {
            foreach(KeyValuePair<EScriptedFunction, string> pair in ScriptPathsForFunctions)
            {
                if (pair.Key == Function)
                    return pair.Value;
            }

            throw new ArgumentException("There is no file associated with function " + Enum.GetName(typeof(EScriptedFunction), Function));
        }

        public static string GetFilePathForOutput(EScriptedFunction Function, Device device)
        {
            return scriptFilesDirectory + device.serial + "_" + Enum.GetName(typeof(EScriptedFunction), Function) + ".txt";
        }

    }
}
