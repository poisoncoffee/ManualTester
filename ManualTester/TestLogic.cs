using System;
using System.Collections.Generic;
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

    }

    public class TestLogic
    {
        public bool forceTest = false;          // If set to true, launch test despite the fact DeviceModel "failed to launch app". Used with assumption that end user launched the app manually.

        private string packagename;
        private Device device;
        private Logcat logcat = new Logcat();

        private struct Coordinates
        {
            public int posX;
            public int posY;
        }

        public TestLogic(string givenPackagename)
        {
            packagename = givenPackagename;
        }

        public void CreateTest(List<TestStep> testStepPlan, Device givenDevice)
        {
            device = givenDevice;
            ExecuteTestSteps(testStepPlan);
            OnTestEnded(TestResultEventArgs.ResultType.Success);
        }


        void ExecuteTestSteps(List<TestStep> testStepsPlan)
        {
            int logcatOffset = 0;

            //begin adb logcat
            logcat = device.logcatOperator.BeginLogcat(packagename);

            //now execute every step
            int testStepIndex = 0;
            while (testStepIndex < testStepsPlan.Count)
            {
                TestStep currentStep = testStepsPlan[testStepIndex];
                bool isStepSuccess = false;
                bool isConditionPresent = false;
                bool isConfirmationPresent = false;
                int alreadyWaitedFor = 0;

                Thread.Sleep(1500);                         //TODO: Individual delay for each step
                ExecuteAction(currentStep);

                while (!isStepSuccess)
                {
                    logcat = device.logcatOperator.UpdateLogcat(logcat, logcatOffset);

                    //Read new logs and check every line
                    //TODO: Blacklist. Check every line for error or exception
                    while (logcatOffset < logcat.logs.Count)
                    {
                        if (!isConditionPresent)
                        {
                            isConditionPresent = IsTargetPresent(logcat.logs[logcatOffset], currentStep.conditionLog);
                            if (isConditionPresent)
                                Thread.Sleep(currentStep.waitTime);
                        }

                        if (isConditionPresent && !isConfirmationPresent)
                        {
                            isConfirmationPresent =  IsTargetPresent(logcat.logs[logcatOffset], currentStep.confirmationLog);
                        }

                        if (isConditionPresent && isConfirmationPresent)
                        {
                            isStepSuccess = true;
                            break;
                        }

                        OnLogRead(logcat.logs[logcatOffset]);
                        logcatOffset++;
                    }

                    //If !isStepSuccess is checked later on because there is a possibility to retry
                    if (isStepSuccess)
                    {
                        OnStepSucceeded(currentStep.id);
                        testStepIndex++;
                        break;
                    }

                    //If no confirmation log was found, wait 500 ms before updating logcat again
                    Thread.Sleep(500);
                    alreadyWaitedFor += 500;

                    if (alreadyWaitedFor > currentStep.terminationTime)     //unless it times out
                    {
                        break;
                    }
                }

                if (!isStepSuccess)
                {
                    switch (currentStep.actionIfFailed)
                    {
                        case TestDatabase.TestAction.Back:
                            device.deviceModel.InputBack();
                            testStepIndex++;
                            break;
                        case TestDatabase.TestAction.BackAndRetry:
                            device.deviceModel.InputBack();
                            break;
                        case TestDatabase.TestAction.Next:
                            OnStepFailed(currentStep.id);
                            testStepIndex++;
                            break;
                        case TestDatabase.TestAction.Retry:
                            break;
                        case TestDatabase.TestAction.Stop:
                            OnStepFailed(currentStep.id);
                            OnTestEnded(TestResultEventArgs.ResultType.StepFailed);
                            testStepIndex = testStepsPlan.Count;
                            break;
                        default:
                            throw new ArgumentException("No behaviour found for this type of action: " + Enum.GetName(typeof(TestDatabase.TestAction), currentStep.actionIfFailed));
                    }

                    //It checks if the device is disconnected to be sure it was not the cause of the failure.
                    if (!device.deviceModel.IsDeviceReady())
                    {
                        OnTestEnded(TestResultEventArgs.ResultType.DeviceDisconnected);
                    }

                }
            }

        }

        private bool IsTargetPresent(string logcatLog, List<string> targetLogs)
        {
            if (targetLogs.Count == 0)
            {
                return true;
            }

            foreach (string targetLog in targetLogs)
            {
                if (logcatLog.Contains(targetLog))
                {
                    return true;
                }
            }
            return false;
        }

        private void ExecuteAction(TestStep step)
        {
            switch (step.testStepType)
            {
                case TestStep.StepType.Tap:
                    Coordinates coordinates = CalculateTapCoordinates(step);
                    device.deviceModel.InputTap(coordinates.posX, coordinates.posY);
                    break;
                case TestStep.StepType.Wait:
                    Thread.Sleep(step.waitTime);
                    break;
                case TestStep.StepType.ExecuteScript:
                    device.deviceModel.ExecuteScript(step.scriptName);
                    break;
                case TestStep.StepType.Back:
                    device.deviceModel.InputBack();
                    break;
                default:
                    throw new NotImplementedException("Behaviour for this Step Type: " + step.testStepType + " is not defined");
            }
        }

        private Coordinates CalculateTapCoordinates(TestStep step)
        {
            Coordinates result = new Coordinates();
            result.posX = (int)(device.resolutionX * step.posX);
            result.posY = (int)(device.resolutionY * step.posY);
            return result;
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
