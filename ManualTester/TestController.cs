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

        private List<TestSequence> testSequencesList = new List<TestSequence>();
        private List<TestStep> choosenTestSteps = new List<TestStep>();
        private List<string> consoleOutputBuffer = new List<string>();
        private bool isUpdatingConsoleOutputAllowed = true;
        private Task test;



        public TestController()
        {
            InitializeComponent();
            testModel = new TestModel();
            testModel.SetCurrentPlatform();
            RefreshDevicesView();
            availableApps.DataSource = Settings.GetAvailableAppsList();
            Settings.SelectApp(availableApps.Text);
        }

        #region ButtonsActions


        //"RUN TEST" button. Starts test without force (force set to false)
        private void runTestButton_Click(object sender, EventArgs e)
        {
            StartTest(false); 
        }

        private void connectedDeviceRefreshButton_Click(object sender, EventArgs e)
        {
            RefreshDevicesView();
        }

        private void loadSequencesButton_Click(object sender, EventArgs e)
        {
            if (!testModel.InitializeDefinitions())
            {
                MessageBox.Show("Please choose the project", "Loading Failed", MessageBoxButtons.OK);
                return;
            }

            List<string> defs = TestDatabase.ConvertIDefinablesToStrings(TestDatabase.sequenceDefinitions);
            DisplayDefinitions(defs);
            List<TestSequence> list = TestDatabase.ConvertStringsToIDefinables(defs, new TestSequence());
            Console.WriteLine(list);

        }

        private void DisplayDefinitions(List<string> sequenceDefinitions)
        {
            foreach (string tsqname in sequenceDefinitions)
            {
                loadedSequences.Items.Add(tsqname);
            }

        }

        private void choosenPlatformAndroid_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("No platform other than Android is supported", "No choice", MessageBoxButtons.OK);
            chosenPlatformAndroid.Checked = true;
        }

        private void addSequence_Click(object sender, EventArgs e)
        {
            if (loadedSequences.SelectedItem != null)
            {
                chosenSequences.Items.Add(loadedSequences.SelectedItem);
            }
        }

        private void removeSequence_Click(object sender, EventArgs e)
        {
            if (chosenSequences.SelectedItem != null)
            {
                chosenSequences.Items.Remove(chosenSequences.SelectedItem);
            }

        }

        private void moveSequenceUp_Click(object sender, EventArgs e)
        {
            if (chosenSequences.SelectedItem != null)
            {

                if (chosenSequences.SelectedIndex != 0)
                {
                    int i = chosenSequences.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on higher position
                    // 3. overwriting item on higher position with variable
                    var selectedItem = chosenSequences.Items[i];
                    chosenSequences.Items[i] = chosenSequences.Items[i - 1];
                    chosenSequences.Items[i - 1] = selectedItem;

                    chosenSequences.SetSelected(i - 1, true);
                }

            }

        }

        private void moveSequenceDown_Click(object sender, EventArgs e)
        {
            if (chosenSequences.SelectedItem != null)
            {

                if (chosenSequences.SelectedIndex != chosenSequences.Items.Count - 1)
                {
                    int i = chosenSequences.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on lower position
                    // 3. overwriting item on lower position with variable
                    var selectedItem = chosenSequences.Items[i];
                    chosenSequences.Items[i] = chosenSequences.Items[i + 1];
                    chosenSequences.Items[i + 1] = selectedItem;

                    chosenSequences.SetSelected(i + 1, true);
                }

            }
        }


        //Pauses updating the consoleOutput window, allowing user to read logs in peace.
        //Meanwhile, the logs will be stored in consoleOutputBuffer and added to consoleOutput window after resume.
        private void consoleOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isUpdatingConsoleOutputAllowed)
            {
                isUpdatingConsoleOutputAllowed = false;
            }
            else if (!isUpdatingConsoleOutputAllowed)
            {
                isUpdatingConsoleOutputAllowed = true;

                foreach (string log in consoleOutputBuffer)
                {
                    UpdateConsoleOutput(log);
                }

                consoleOutputBuffer.Clear();
            }
        }

        private void comboBoxWithAvailableApps_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Settings.SelectApp(availableApps.SelectedItem.ToString());
        }
        #endregion

        #region ActionsDefinitions

        private void RefreshDevicesView()
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

        private void SetWindowEnabled(bool enabled)
        {
            foreach (Control c in this.Controls)
            {
                if (c.Enabled != enabled)
                {
                    c.Enabled = enabled;
                }
            }
        }

        private void UpdateConsoleOutput(string log)
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
                choosenTestSteps = TestDatabase.ConvertStringsToIDefinables(chosenSequences.Items.Cast<string>().ToList(), new TestStep());
            }
            else
            {
                MessageBox.Show("Please select at least one step action to execute", "Unable to start test", MessageBoxButtons.OK);
                return;
            }

            FillChoosenStepStatusCheckedList(choosenTestSteps);

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

        private void FillChoosenStepStatusCheckedList(List<TestStep> testStepList)
        {
            stepsStatus.Items.Clear();

            foreach (var ts in testStepList)
            {
                stepsStatus.Items.Add(ts.testStepID);
            }
        }

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
                if (isUpdatingConsoleOutputAllowed)
                {
                    UpdateConsoleOutput(log);
                }
                else if (!isUpdatingConsoleOutputAllowed)
                {
                    consoleOutputBuffer.Add(log);
                }
            }
        }



        #endregion


    }
}
