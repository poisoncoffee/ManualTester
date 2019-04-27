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

    public class TestManager
    {
        public bool testsCancelled = false;
        public bool forceTest = false;          // if set to true, launches test despite the fact DeviceModel failed to launch app. Used with assumption that end user launched the app manually.


        private string packagename;
        private string processPID;
        private string executedDirectoryPath; 

        private List<TestStep> testStepDefinitions = new List<TestStep>();
        private List<TestSequence> testSequenceDefinitions = new List<TestSequence>();

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

                        //loading tests definitions
                        TestDatabase testDatabase = TestDatabase.Instance;
                        testStepDefinitions = testDatabase.LoadTestStepDefinitions(packagename);
                        testSequenceDefinitions = testDatabase.LoadTestSequenceDefinitions(packagename);

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
            //begin adb logcat
            DeviceModel.Logcat logcat = DeviceModel.BeginLogcat(processPID, packagename);
            logcat = DeviceModel.UpdateLogcat(logcat, 0);

            int logcatOffset = 0;
            foreach(TestStep step in testStepsPlan)
            {
                bool isConditionPresent = false;
                while (!isConditionPresent)
                {
                    //open logcat

                    long lastPosition = 0;

                   /* FileStream file = new FileStream("dd.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    file.Position
                    file.Seek
                    file.
                    
                   

                    file
                    {
                        
                    }
                    */

                }


                    if (true) //if step conditionlog is present
                    {
                        DeviceModel.InputTap(step.posX, step.posY);
                    }
                
            }
        }

        #region Events

        public delegate void StepSucceededEventHandler(object sender, EventArgs e);
        public event StepSucceededEventHandler StepSucceeded;
        protected virtual void OnStepSucceeded()
        {
            if (StepSucceeded != null)
            {
                StepSucceeded(this, EventArgs.Empty);
            }
        }

        public delegate void TestEndedEventHandler(object sender, TestEventArgs e);
        public event TestEndedEventHandler TestEnded;
        protected virtual void OnTestEnded(string result)
        {
            if (TestEnded != null)
            {
                TestEnded(this, new TestEventArgs() {argument = result });
            }
        }

        public delegate void DeviceNotConnectedEventHandler(object sender, TestEventArgs e);
        public event DeviceNotConnectedEventHandler DeviceNotConnected;
        protected virtual void OnDeviceNotConnected(string packagename)
        {
            if (DeviceNotConnected != null)
            {
                DeviceNotConnected(this, new TestEventArgs() { argument = packagename });
            }
        }


        public delegate void AppLaunchFailedEventHandler(object sender, TestEventArgs e);
        public event AppLaunchFailedEventHandler AppLaunchFailed;
        protected virtual void OnAppLaunchFailed(string packagename)
        {
            if (AppLaunchFailed != null)
            {
                AppLaunchFailed(this, new TestEventArgs() { argument = packagename });
            }
        }

        #endregion


    }
         
}
