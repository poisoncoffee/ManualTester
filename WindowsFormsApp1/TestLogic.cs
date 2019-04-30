﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading;

namespace WindowsFormsApp1
{
    public class TestEventArgs : EventArgs
    {
        public string argument { get; set; }
    }

    public class TestResultEventArgs : EventArgs
    {
        public enum ResultType
        {
            Success,
            StepFailed,
            DeviceDisconnected,
            AppLaunchFailed,
            LoadingDefinitionsFailed
        }

        public ResultType resultType { get; set; }
        public int numSucceeded { get; set; }
        public int numFailed { get; set; }
    }

    public class TestLogic
    {
        public bool testsCancelled = false;
        public bool forceTest = false;          // if set to true, launches test despite the fact DeviceModel failed to launch app. Used with assumption that end user launched the app manually.


        private string packagename;
        private string processPID;
        private string executedDirectoryPath; 

        private List<TestStep> testStepDefinitions = new List<TestStep>();
        private List<TestSequence> testSequenceDefinitions = new List<TestSequence>();

        private DeviceModel.Logcat logcat = new DeviceModel.Logcat();

        public TestLogic(string givenPackagename)
        {
            packagename = givenPackagename;
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public void CreateTest(List<TestStep> testStepPlan)
        {

            if(DeviceModel.IsDeviceReady())
            {

                    if(DeviceModel.LaunchApp(packagename) || forceTest)
                    {
                    processPID = DeviceModel.GetProcessPID(packagename);

                    ExecuteTestSteps(testStepPlan);            
                    } 
                    else
                    {
                        OnAppLaunchFailed(packagename);
                    }
            }
            else
            {
                OnDeviceNotConnected(packagename);
            }

        }



        void ExecuteTestSteps(List<TestStep> testStepsPlan)
        {
            int logcatOffset = 0;

            //begin adb logcat
            logcat = DeviceModel.BeginLogcat(processPID, packagename);

            //now execute every step
            foreach(TestStep step in testStepsPlan)
            {
                ExecuteStep:
                bool isStepSuccess = false;
                bool isConditionPresent = false;
                bool isConfirmationPresent = false;
                int alreadyWaitedFor = 0;

                //execute tap
                TestStep step2 = step;
                Thread.Sleep(2000);
                DeviceModel.InputTap(step.posX, step.posY);

                while (!isStepSuccess)
                {
                    logcat = DeviceModel.UpdateLogcat(logcat, logcatOffset);

                    //read log and check every line
                    while (logcatOffset < logcat.logs.Count)
                    {
                        if (!isConditionPresent)
                        {
                            if (step.conditionLog.Count == 0)
                            {
                                isConditionPresent = true;
                            }
                            else
                            {
                                isConditionPresent = IsLogPresent(logcat.logs[logcatOffset], step.conditionLog);
                            }
                        }

                        if (isConditionPresent && !isConfirmationPresent)
                        {
                            if (step.confirmationLog.Count == 0)
                            {
                                isConfirmationPresent = true;
                            }
                            else
                            {
                                isConfirmationPresent = IsLogPresent(logcat.logs[logcatOffset], step.confirmationLog);
                            }
                        }

                        if (isConditionPresent && isConfirmationPresent)
                        {
                            isStepSuccess = true;
                            break;
                        }

                        string log = "[" + logcatOffset + "]" + logcat.logs[logcatOffset];
                        OnLogRead(log);
                        logcatOffset++;

                    }

                    //if isStepFailed is checked later on because there is a possibility to retry
                    if (isStepSuccess)
                    {
                        OnStepSucceeded(step.testStepID);
                        break;
                    }

                    //If no confirmation log was found, wait 500 ms before updating logcat again
                    Thread.Sleep(500);
                    alreadyWaitedFor += 500;

                    if (alreadyWaitedFor > step.terminationTime)     //unless it times out
                    {
                        break;
                    }
                }  
                if(!isStepSuccess)
                {
                    TestStep step3 = step;
                    switch (step.actionIfFailed)
                    {
                        case TestDatabase.TestAction.Back:
                            DeviceModel.InputBack();
                            break;
                        case TestDatabase.TestAction.BackAndRetry:
                            DeviceModel.InputBack();
                            goto ExecuteStep;
                        case TestDatabase.TestAction.Next:
                            OnStepFailed(step.testStepID);
                            break;
                        case TestDatabase.TestAction.Retry:
                            goto ExecuteStep;
                        case TestDatabase.TestAction.Stop:
                            OnStepFailed(step.testStepID);
                            OnTestEnded(TestResultEventArgs.ResultType.StepFailed);
                            break;
                        default:
                            goto case TestDatabase.TestAction.Stop;
                    }

                    //It checks if the device is disconnected to be sure it was not the cause of the failure.
                    if (!DeviceModel.IsDeviceReady())
                    {
                        OnTestEnded(TestResultEventArgs.ResultType.DeviceDisconnected);
                    }
                }
            }
        }

        private bool IsLogPresent(string logcatLog, List<string> targetLogs)
        {
            foreach (string targetLog in targetLogs)
            {
                if (logcatLog.Contains(targetLog))
                {
                    return true;
                }
            }
            return false;
        }


        #region Events

        public delegate void StepSucceededEventHandler(object sender, TestEventArgs e);
        public event StepSucceededEventHandler StepSucceeded;
        protected virtual void OnStepSucceeded(string stepName)
        {
            if(StepSucceeded != null)
                StepSucceeded(this, new TestEventArgs() { argument = stepName });
        }

        public delegate void StepFailedEventHandler(object sender, TestEventArgs e);
        public event StepFailedEventHandler StepFailed;
        protected virtual void OnStepFailed(string stepName)
        {
            if(StepFailed != null)
                StepFailed(this, new TestEventArgs() { argument = stepName });
        }

        public delegate void TestEndedEventHandler(object sender, TestResultEventArgs e);
        public event TestEndedEventHandler TestEnded;
        protected virtual void OnTestEnded(TestResultEventArgs.ResultType result)
        {
            if(TestEnded != null)
                TestEnded(this, new TestResultEventArgs() { resultType = result });
        }

        public delegate void DeviceNotConnectedEventHandler(object sender, TestEventArgs e);
        public event DeviceNotConnectedEventHandler DeviceNotConnected;
        protected virtual void OnDeviceNotConnected(string packagename)
        {
            if(DeviceNotConnected != null)
                DeviceNotConnected(this, new TestEventArgs() { argument = packagename });
        }


        public delegate void AppLaunchFailedEventHandler(object sender, TestEventArgs e);
        public event AppLaunchFailedEventHandler AppLaunchFailed;
        protected virtual void OnAppLaunchFailed(string packagename)
        {
            if(AppLaunchFailed != null)
                AppLaunchFailed(this, new TestEventArgs() { argument = packagename });
        }


        public delegate void LogReadEventHandler(object sender, TestEventArgs e);
        public event LogReadEventHandler LogRead;
        protected virtual void OnLogRead(string log)
        {
            if (LogRead != null)
                LogRead(this, new TestEventArgs() { argument = log });
        }

        #endregion


    }
         
}
