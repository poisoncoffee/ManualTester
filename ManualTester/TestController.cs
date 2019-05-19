using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{
    public partial class TestController : Form
    {
        private List<TestSequence> testSequencesList = new List<TestSequence>();
        private List<TestStep> choosenTestSteps = new List<TestStep>();
        private List<string> consoleOutputBuffer = new List<string>();
        private string currentProjectsPackagename = "";
        private bool isUpdatingConsoleOutputAllowed = true;
        private Task test;
        private IDeviceModel currentPlatform;


        public TestController()
        {
            InitializeComponent();
            SetCurrentPlatform();
            RefreshDevices();
            comboBoxWithAvailableApps.DataSource = GetAvailableAppsList();
        }

        #region ButtonsActions


        //"RUN TEST" button. Starts test without force (force set to false)
        private void runTestButton_Click(object sender, EventArgs e)
        {
            StartTest(false); 
        }

        private void connectedDeviceRefreshButton_Click(object sender, EventArgs e)
        {
            RefreshDevices();
        }

        private void loadSequencesButton_Click(object sender, EventArgs e)
        {
            if (comboBoxWithAvailableApps.Text != null)
            {
                currentProjectsPackagename = comboBoxWithAvailableApps.Text;
            }
            else
            {
                MessageBox.Show("Please choose the project", "Loading Failed", MessageBoxButtons.OK);
                return;
            }

            //TODO: Do this in separate Initializer class
            TestDatabase database = TestDatabase.Instance;
            database.LoadTestSequenceDefinitions(currentProjectsPackagename);
            database.LoadTestStepDefinitions(currentProjectsPackagename);
            List<string> sequenceDefinitionsList = database.GetSequenceDefinitionsAsString();
            sequenceDefinitionsListBox.Items.Clear();

            foreach(string tsqname in sequenceDefinitionsList)
            {
                sequenceDefinitionsListBox.Items.Add(tsqname);
            }

        }

        private void choosenPlatformAndroid_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show("No platform other than Android is supported", "No choice", MessageBoxButtons.OK);
            choosenPlatformAndroid.Checked = true;
        }

        private void addSequence_Click(object sender, EventArgs e)
        {
            if (sequenceDefinitionsListBox.SelectedItem != null)
            {
                choosenSequenceListBox.Items.Add(sequenceDefinitionsListBox.SelectedItem);
            }
        }

        private void removeSequence_Click(object sender, EventArgs e)
        {
            if (choosenSequenceListBox.SelectedItem != null)
            {
                choosenSequenceListBox.Items.Remove(choosenSequenceListBox.SelectedItem);
            }

        }

        private void moveSequenceUp_Click(object sender, EventArgs e)
        {
            if (choosenSequenceListBox.SelectedItem != null)
            {

                if (choosenSequenceListBox.SelectedIndex != 0)
                {
                    int i = choosenSequenceListBox.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on higher position
                    // 3. overwriting item on higher position with variable
                    var selectedItem = choosenSequenceListBox.Items[i];
                    choosenSequenceListBox.Items[i] = choosenSequenceListBox.Items[i - 1];
                    choosenSequenceListBox.Items[i - 1] = selectedItem;

                    choosenSequenceListBox.SetSelected(i - 1, true);
                }

            }

        }

        private void moveSequenceDown_Click(object sender, EventArgs e)
        {
            if (choosenSequenceListBox.SelectedItem != null)
            {

                if (choosenSequenceListBox.SelectedIndex != choosenSequenceListBox.Items.Count - 1)
                {
                    int i = choosenSequenceListBox.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on lower position
                    // 3. overwriting item on lower position with variable
                    var selectedItem = choosenSequenceListBox.Items[i];
                    choosenSequenceListBox.Items[i] = choosenSequenceListBox.Items[i + 1];
                    choosenSequenceListBox.Items[i + 1] = selectedItem;

                    choosenSequenceListBox.SetSelected(i + 1, true);
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
        #endregion

        #region ActionsDefinitions

        //Placeholder, TODO. Currently returns "com.artifexmundi.balefire" as this is the app I test. In the future will search for available apps in some config file.
        private List<string> GetAvailableAppsList()
        {
            List<string> AppList = new List<string>();
            AppList.Add("com.artifexmundi.balefire");
            return AppList;            
        }

        private void RefreshDevices()
        {
            List<Device> ConnectedDevices = currentPlatform.GetConnectedDevices();
            if(ConnectedDevices.Count == 0)
            {
                connectedDeviceIDLabel.Text = "No devices connected";
            }
            else if (ConnectedDevices.Count == 1)
            {
                connectedDeviceIDLabel.Text = ConnectedDevices[0].serial + " " + ConnectedDevices[0].status;
            }
            else
            {
                connectedDeviceIDLabel.Text = "More than one device connected";
            }

        }

        //Changes all controls in the window to enabled or disabled
        private void ChangeEnabled(bool enabled)
        {
            foreach (Control c in this.Controls)
            {
                if (c.Enabled != enabled)
                {
                    c.Enabled = enabled;
                }
            }
        }

        //TODO. Temporary solution - only one platform is supported right now
        private void SetCurrentPlatform()
        {
            currentPlatform = new ADBWrapper();

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
            string packagename = comboBoxWithAvailableApps.Text;
            TestDatabase database = TestDatabase.Instance;
            TestLogic testLogic = new TestLogic(packagename);
            List<Device> ReadyDevices = new List<Device>();

            if (choosenSequenceListBox.Items.Count != 0)
            {
                choosenTestSteps = database.ConvertSequencesAsStringToSteps(choosenSequenceListBox.Items.Cast<string>().ToList());
            }
            else
            {
                MessageBox.Show("Please select at least one step action to execute", "Unable to start test", MessageBoxButtons.OK);
                return;
            }

            FillChoosenStepStatusCheckedList(choosenTestSteps);

            //check the devices
            if(currentPlatform.IsDeviceReady())
            {
                ReadyDevices = currentPlatform.GetReadyDevicesFullInfo();
            }
            else
            {
                MessageBox.Show("No device able to run the test", "Unable to start test", MessageBoxButtons.OK);
                return;
            }

            //subscribe to all events
            testLogic.TestEnded += OnTestEnded;
            testLogic.StepSucceeded += OnStepSucceeded;
            testLogic.DeviceNotConnected += OnDeviceNotConnected;
            testLogic.AppLaunchFailed += OnAppLaunchFailed;
            testLogic.LogRead += OnLogRead;

            //force set to true launches test despite fact that app could not be opened via adb - assumes the user launched the app manually. See OnAppLaunchFailed event
            if (force)
                testLogic.forceTest = true;

            test = new Task(() => { testLogic.CreateTest(choosenTestSteps, ReadyDevices[0]); } ); //TODO: Now supporting only one device
            test.Start();
        }

        private void FillChoosenStepStatusCheckedList(List<TestStep> testStepList)
        {
            choosenStepsStatusCheckedList.Items.Clear();

            foreach (var ts in testStepList)
            {
                choosenStepsStatusCheckedList.Items.Add(ts.testStepID);
            }

        }

        // TODO
        public void EndTest()
        {
            ChangeEnabled(true);
        }
        #endregion

        #region TestRelatedEvents

        public void OnStepSucceeded(object sender, TestEventArgs e)
        {
            if(choosenStepsStatusCheckedList.InvokeRequired)
            {
                TestEventArgs arg = new TestEventArgs() { argument = e.argument };
                choosenStepsStatusCheckedList.Invoke(new TestLogic.StepSucceededEventHandler(OnStepSucceeded), sender, e);
            }
            else
            {
                for(int i = 0; i < choosenStepsStatusCheckedList.Items.Count; i++)
                {
                    if (choosenStepsStatusCheckedList.Items[i].ToString() == e.argument && !choosenStepsStatusCheckedList.GetItemChecked(i))
                    {
                        choosenStepsStatusCheckedList.SetItemChecked(i, true);
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
