using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{
    public partial class TestController : Form
    {

        private TestModel testModel;

        private List<TestSet> testSequencesList = new List<TestSet>();
        private List<TestStep> choosenTestSteps = new List<TestStep>();
        private List<string> consoleOutputBuffer = new List<string>();
        private bool isConsoleLocked = true;
        private Task test;



        public TestController()
        {
            InitializeComponent();
            testModel = new TestModel();
            testModel.SetCurrentPlatform();
            UpdateDevices();
            availableApps.DataSource = Settings.GetAvailableAppsList();
            Settings.SelectApp(availableApps.Text);
        }

        #region ButtonsActions


        //"RUN TEST" button. Starts test without force (force set to false)
        private void runTestButton_Click(object sender, EventArgs e)
        {
            StartTest(false); 
        }

        //REAFCTORED
        private void updateDevices_Click(object sender, EventArgs e)
        {
            UpdateDevices();
        }

        //REFACTORED
        private void loadSequencesButton_Click(object sender, EventArgs e)
        {
            testModel.InitializeDefinitions();
            DisplayDefinitions();
        }

        //REFACTORED
        private void DisplayDefinitions()
        {
            loadedSequences.DataSource = testModel.GetAvailableSequences();
        }

        //REFACTORED
        private void DisplaySteps(List<TestStep> testStepList)
        {
            stepsStatus.Items.Clear();
            testStepList.ForEach(step => { stepsStatus.Items.Add(step.GetID()); } );
        }

        //FUTURE MILESTONE
        private void choosenPlatformAndroid_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("No platform other than Android is supported", "No choice", MessageBoxButtons.OK);
            chosenPlatformAndroid.Checked = true;
        }

        //REFACTORED
        private void addSequence_Click(object sender, EventArgs e)
        {
            if (loadedSequences.SelectedItem != null)
                chosenSequences.Items.Add(loadedSequences.SelectedItem);
        }

        //REFACTORED
        private void removeSequence_Click(object sender, EventArgs e)
        {
            if (chosenSequences.SelectedItem != null)
                chosenSequences.Items.Remove(chosenSequences.SelectedItem);
        }

        //REFACTORED
        private void moveSequenceUp_Click(object sender, EventArgs e)
        {
            MoveSelectedSequence(1);
        }

        //REFACTORED
        private void moveSequenceDown_Click(object sender, EventArgs e)
        {
            MoveSelectedSequence(-1);
        }

        //REFACTORED
        private void MoveSelectedSequence(int offset)
        {
            if(chosenSequences.SelectedItem!= null)
            {
                int i = chosenSequences.SelectedIndex - offset;
                if (i >= 0 && i < chosenSequences.Items.Count)
                {
                    var selectedItem = chosenSequences.Items[i + offset];
                    chosenSequences.Items[i + offset] = chosenSequences.Items[i];
                    chosenSequences.Items[i] = selectedItem;
                    chosenSequences.SetSelected(i, true);
                }
            }
        }

        //REFACTORED
        //Pauses updating the consoleOutput window, allowing user to read logs in peace.
        //Meanwhile, the logs will be stored in consoleOutputBuffer and added to consoleOutput window after resume.
        private void consoleOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            isConsoleLocked = !isConsoleLocked;
            if (isConsoleLocked)
            {
                foreach (string log in consoleOutputBuffer)
                {
                    AppendToConsole(log);
                }
                consoleOutputBuffer.Clear();
            }
        }

        //REFACTORED
        private void comboBoxWithAvailableApps_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Settings.SelectApp(availableApps.SelectedItem.ToString());
        }
        #endregion

        #region ActionsDefinitions

        //FUTURE MILESTONE
        //Support for multiple devices is a TODO. Right now this is a placeholder behaviour.
        private void UpdateDevices()
        {
            List<Device> ConnectedDevices = testModel.currentPlatform.GetConnectedDevices();
            if(ConnectedDevices.Count == 0)
            {
                deviceStatusLabel.Text = "No devices connected";
            }
            else if (ConnectedDevices.Count == 1)
            {
                deviceStatusLabel.Text = ConnectedDevices[0].serial + " " + ConnectedDevices[0].status;
            }
            else
            {
                deviceStatusLabel.Text = "More than one device connected";
            }

        }

        //REFACTORED
        private void SetWindowEnabled(bool enabled)
        {
            foreach (Control c in Controls)
            {
                c.Enabled = enabled;
            }
        }

        //REFACTORED
        private void AppendToConsole(string log)
        {
            consoleOutput.AppendText(log);
            consoleOutput.Text += Environment.NewLine;
            consoleOutput.SelectionStart = consoleOutput.Text.Length;
            consoleOutput.ScrollToCaret();
        }

        //The logic is:
        // 1. Check if any sequence has been choosen - if yes, convert them to TestSteps and fill the checked list with them
        // 2. Subscribe to all events
        // 3. Check the device
        // 4. Start new Task
        private void StartTest(bool force)
        {
            TestLogic testLogic = new TestLogic(Settings.chosenApp);
            List<Device> ReadyDevices = new List<Device>();

            if (chosenSequences.Items.Count != 0)
            {
                //WIP Solution: will be reworked
                List<TestSet> seqs = TestDatabase.ConvertStringsToIDefinables(chosenSequences.Items.Cast<string>().ToList(), new TestSet());
                
                foreach (TestSet sequence in seqs)
                {
                   choosenTestSteps.AddRange(sequence.Flatify());
                }
            }
            else
            {
                MessageBox.Show("Please select at least one step action to execute", "Unable to start test", MessageBoxButtons.OK);
                return;
            }

            DisplaySteps(choosenTestSteps);

            //check the devices
            if(testModel.currentPlatform.IsDeviceReady())
            {
                ReadyDevices = testModel.currentPlatform.GetReadyDevicesFullInfo();
            }
            else
            {
                MessageBox.Show("No device able to run the test", "Unable to start test", MessageBoxButtons.OK);
                return;
            }

            //subscribe to all events
            testLogic.TestEnded += OnTestEnded;
            testLogic.StepSucceeded += OnStepSucceeded;
            testLogic.LogRead += OnLogRead;

            //force set to true launches test despite fact that app could not be opened via adb - assumes the user launched the app manually. See OnAppLaunchFailed event
            if (force)
                testLogic.forceTest = true;

            test = new Task(() => { testLogic.CreateTest(choosenTestSteps, ReadyDevices[0]); } ); //TODO: Now supporting only one device
            test.Start();
        }



        //FUTURE MILESTONE
        // TODO
        public void EndTest()
        {
            SetWindowEnabled(true);
        }
        #endregion

        #region TestRelatedEvents

        public void OnStepSucceeded(object sender, TestEventArgs e)
        {
            if(stepsStatus.InvokeRequired)
            {
                TestEventArgs arg = new TestEventArgs() { argument = e.argument };
                stepsStatus.Invoke(new TestLogic.StepSucceededEventHandler(OnStepSucceeded), sender, e);
            }
            else
            {
                for(int i = 0; i < stepsStatus.Items.Count; i++)
                {
                    if (stepsStatus.Items[i].ToString() == e.argument && !stepsStatus.GetItemChecked(i))
                    {
                        stepsStatus.SetItemChecked(i, true);
                        break;
                    }
                }

            }
        }

        public void OnTestEnded(object sender, TestResultEventArgs e)
        {
            string message = "Test Ended not Implemented yet, reason: " + e.resultType.ToString();
            MessageBox.Show(message, "Test Ended", MessageBoxButtons.OK);
            EndTest();
        }

        public void OnDeviceNotConnected(object sender, TestEventArgs e)
        {
            MessageBox.Show("Device is not connected or unauthenticated.\nEnsure USB debugging is enabled", "Test Ended", MessageBoxButtons.OK);
            EndTest();
        }

        public void OnAppLaunchFailed(object sender, TestEventArgs e)
        {
            DialogResult result = MessageBox.Show("Failed to launch app.\nYou can launch the app manually and select Retry, or Cancel the test", "Manual Launch", MessageBoxButtons.RetryCancel);
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                EndTest();
            }
            else
            {
                StartTest(true);
            }
        }

        public void OnLogRead(object sender, TestEventArgs e)
        {
            string log = string.Copy(e.argument);
            if (consoleOutput.InvokeRequired)
            {
                TestEventArgs arg = new TestEventArgs() { argument = e.argument };
                consoleOutput.Invoke(new TestLogic.LogReadEventHandler(OnLogRead), sender, e);
            }
            else
            {
                if (isConsoleLocked)
                {
                    AppendToConsole(log);
                }
                else if (!isConsoleLocked)
                {
                    consoleOutputBuffer.Add(log);
                }
            }
        }




        #endregion

    }
}
