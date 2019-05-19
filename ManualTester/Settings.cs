﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Settings
    {
        static string executedDirectoryPath;

        public static void InitializeSettings()
        {
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static string GetExecutedDirectoryPath()
        {
            return executedDirectoryPath;
        }
    }
}
