using System;
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

    public class TestManager
    {
        public bool testsCancelled = false;
        public bool forceTest = false;          // if set to true, launches test despite the fact DeviceModel failed to launch app. Used with assumption that end user launched the app manually.


        private string packagename;
        private string processPID;
        private string executedDirectoryPath; 

        private List<TestStep> testStepDefinitions = new List<TestStep>();
        private List<TestSequence> testSequenceDefinitions = new List<TestSequence>();

        private DeviceModel.Logcat logcat = new DeviceModel.Logcat();

        public TestManager(string givenPackagename)
        {
            packagename = givenPackagename;
            executedDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public void CreateTest(List<string> choosenSequences)
        {

            if(DeviceModel.IsDeviceReady())
            {

                    if(DeviceModel.LaunchApp(packagename) || forceTest)
                    {
                    processPID = DeviceModel.GetProcessPID(packagename);
                    TestDatabase testDatabase = TestDatabase.Instance;

                    try
                    {
                        //loading tests definitions
                        testStepDefinitions = testDatabase.LoadTestStepDefinitions(packagename);
                        testSequenceDefinitions = testDatabase.LoadTestSequenceDefinitions(packagename);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        OnTestEnded(TestResultEventArgs.ResultType.LoadingDefinitionsFailed);
                    }
                    catch (System.IO.IOException)
                    {
                        OnTestEnded(TestResultEventArgs.ResultType.LoadingDefinitionsFailed);
                    }

                        List<TestStep> testStepPlan = new List<TestStep>();
                        List<TestSequence> testSequencePlan = new List<TestSequence>();
                        testSequencePlan = ConvertStringsToSequences(choosenSequences);
                        testStepPlan = ConvertTestSequencesToTestSteps(testSequencePlan);

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

        //converting choosenSequences (which are strings) to actual TestSequence objects by name
        List<TestSequence> ConvertStringsToSequences(List<string> choosenSequences)
        {
            List<TestSequence> sequencePlan = new List<TestSequence>();
            foreach (string sequenceName in choosenSequences)
            {
                foreach (TestSequence testSequence in testSequenceDefinitions)
                {
                    if (sequenceName == testSequence.testSequenceID)
                    {
                        sequencePlan.Add(testSequence);
                        break;
                    }
                }
            }

            return sequencePlan;
        }

        List<TestStep> ConvertTestSequencesToTestSteps(List<TestSequence> sequencePlan)
        {
            int count = 0;
            List<TestStep> stepPlan = new List<TestStep>();
            foreach (TestSequence testSequence in sequencePlan)
            {
                foreach (string testStepOnList in testSequence.StepList)
                {
                    foreach (TestStep testStepDefinition in testStepDefinitions)
                    {
                        if (testStepOnList == testStepDefinition.testStepID)
                        {
                            stepPlan.Add(testStepDefinition);
                            count++;
                            break;
                        }
                    }
                }

            }
            return stepPlan;
        }

        void ExecuteTestSteps(List<TestStep> testStepsPlan)
        {
            int logcatOffset = 0;

            //begin adb logcat
            logcat = DeviceModel.BeginLogcat(processPID, packagename);

            //now execute every step
            foreach(TestStep step in testStepsPlan)
            {
                bool isStepSuccess = false;
                int alreadyWaitedFor = 0;

                //execute tap
                DeviceModel.InputTap(step.posX, step.posY);

                while (!isStepSuccess)
                {
                    logcat = DeviceModel.UpdateLogcat(logcat, logcatOffset);

                    //read log and check every line
                    while (logcatOffset < logcat.logs.Count)
                    {
                        OnLogRead(logcat.logs[logcatOffset]);

                        foreach (string confirmation in step.confirmationLog)
                        {
                            if (logcat.logs[logcatOffset].Contains(confirmation))
                            {
                                isStepSuccess = true;
                                logcatOffset++;
                                goto StepSuccess;
                            }
                        }

                        logcatOffset++;
                    }

                    //If no confirmation log was found, wait 50 ms before updating logcat again
                    Thread.Sleep(100);
                    alreadyWaitedFor += 100;
                    if (alreadyWaitedFor > step.terminationTime)     //unless it times out
                    {
                        goto StepFailed;
                    }
                }

                StepSuccess:
                {
                    if(isStepSuccess)
                    {
                        ReactToStepSuccess(step);
                    }
                }

                StepFailed:
                {
                    if (!isStepSuccess)
                    {
                        ReactToStepFailure(step);

                        //It checks if the device is disconnected to be sure it was not the cause of the failure.
                        if (!DeviceModel.IsDeviceReady())
                        {
                            OnTestEnded(TestResultEventArgs.ResultType.DeviceDisconnected);
                        }
                    }
                }
           
           }
        }

        private void ReactToStepSuccess (TestStep step)
        {
            OnStepSucceeded(step.testStepID);
        }

        private void ReactToStepFailure (TestStep step)
        {

        }

        #region Events

        public delegate void StepSucceededEventHandler(object sender, TestEventArgs e);
        public event StepSucceededEventHandler StepSucceeded;
        protected virtual void OnStepSucceeded(string stepName)
        {
                StepSucceeded(this, new TestEventArgs() { argument = stepName });
        }

        public delegate void StepFailedEventHandler(object sender, TestEventArgs e);
        public event StepFailedEventHandler StepFailed;
        protected virtual void OnStepFailed(string stepName)
        {
                StepFailed(this, new TestEventArgs() { argument = stepName });
        }

        public delegate void TestEndedEventHandler(object sender, TestResultEventArgs e);
        public event TestEndedEventHandler TestEnded;
        protected virtual void OnTestEnded(TestResultEventArgs.ResultType result)
        {
                TestEnded(this, new TestResultEventArgs() { resultType = result });
        }

        public delegate void DeviceNotConnectedEventHandler(object sender, TestEventArgs e);
        public event DeviceNotConnectedEventHandler DeviceNotConnected;
        protected virtual void OnDeviceNotConnected(string packagename)
        {
                DeviceNotConnected(this, new TestEventArgs() { argument = packagename });
        }


        public delegate void AppLaunchFailedEventHandler(object sender, TestEventArgs e);
        public event AppLaunchFailedEventHandler AppLaunchFailed;
        protected virtual void OnAppLaunchFailed(string packagename)
        {
                AppLaunchFailed(this, new TestEventArgs() { argument = packagename });
        }


        public delegate void LogReadEventHandler(object sender, TestEventArgs e);
        public event LogReadEventHandler LogRead;
        protected virtual void OnLogRead(string log)
        {
                LogRead(this, new TestEventArgs() { argument = log });
        }

        #endregion


    }
         
}
