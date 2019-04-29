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
        private List<string> choosenSequencesList = new List<string>();
        private List<string> consoleOutputBuffer = new List<string>();
        private bool isUpdatingConsoleOutputAllowed = true;
        private Task test;


        public TestController()
        {
            InitializeComponent();
            RefreshDevices();
            comboBoxWithAvailableApps.DataSource = GetAvailableAppsList();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        #region ButtonsActions


        //"RUN TEST" button. Starts test without force (force set to false)
        private void button1_Click(object sender, EventArgs e)
        {
            StartTest(false); 
        }

        private void comboBoxWithAvailableApps_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            RefreshDevices();
        }

        private void loadSequencesButton_Click(object sender, EventArgs e)
        {
            TestDatabase testDatabase = TestDatabase.Instance;
            testSequencesList = testDatabase.LoadTestSequenceDefinitions("com.artifexmundi.balefire");

            sequenceDefinitionsList.Items.Clear();
            if (testSequencesList.Count != 0)
            {
                foreach (TestSequence tsq in testSequencesList)
                {
                    sequenceDefinitionsList.Items.Add(tsq.testSequenceID);
                }
            }

        }


        private void addSequence_Click(object sender, EventArgs e)
        {
            if (sequenceDefinitionsList.SelectedItem != null)
            {
                choosenSequenceList.Items.Add(sequenceDefinitionsList.SelectedItem);
            }
        }

        private void removeSequence_Click(object sender, EventArgs e)
        {
            if (choosenSequenceList.SelectedItem != null)
            {
                choosenSequenceList.Items.Remove(choosenSequenceList.SelectedItem);
            }

        }

        private void moveSequenceUp_Click(object sender, EventArgs e)
        {
            if (choosenSequenceList.SelectedItem != null)
            {

                if (choosenSequenceList.SelectedIndex != 0)
                {
                    int i = choosenSequenceList.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on higher position
                    // 3. overwriting item on higher position with variable
                    var selectedItem = choosenSequenceList.Items[i];
                    choosenSequenceList.Items[i] = choosenSequenceList.Items[i - 1];
                    choosenSequenceList.Items[i - 1] = selectedItem;

                    choosenSequenceList.SetSelected(i - 1, true);
                }

            }

        }

        private void moveSequenceDown_Click(object sender, EventArgs e)
        {
            if (choosenSequenceList.SelectedItem != null)
            {

                if (choosenSequenceList.SelectedIndex != choosenSequenceList.Items.Count - 1)
                {
                    int i = choosenSequenceList.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on lower position
                    // 3. overwriting item on lower position with variable
                    var selectedItem = choosenSequenceList.Items[i];
                    choosenSequenceList.Items[i] = choosenSequenceList.Items[i + 1];
                    choosenSequenceList.Items[i + 1] = selectedItem;

                    choosenSequenceList.SetSelected(i + 1, true);
                }

            }
        }
        #endregion


        #region ActionsDefinitions
        private List<string> GetAvailableAppsList()
        {
            //not implemented
            List<string> AppList = new List<string>();
            AppList.Add("com.artifexmundi.balefire");
            return AppList;
            
        }

        private void RefreshDevices()
        {
            connectedDeviceIDLabel.Text = DeviceModel.GetDeviceStatus();
        }

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


        private void UpdateConsoleOutput(string log)
        {
            consoleOutput.AppendText(log);
            consoleOutput.Text += Environment.NewLine;
            consoleOutput.SelectionStart = consoleOutput.Text.Length;
            consoleOutput.ScrollToCaret();
        }

        private void StartTest(bool force)
        {

           // ChangeEnabled(false);

            string packagename = comboBoxWithAvailableApps.Text;
            var testManager = new TestManager(packagename);

            //subscribe to all events
            testManager.TestEnded += OnTestEnded;
            testManager.StepSucceeded += OnStepSucceeded;
            testManager.DeviceNotConnected += OnDeviceNotConnected;
            testManager.AppLaunchFailed += OnAppLaunchFailed;
            testManager.LogRead += OnLogRead;

            //force set to true launches test despite fact that app could not be opened via adb - assumes the user launched the app manually. See OnAppLaunchFailed event
            if (force)
                testManager.forceTest = true;

            choosenSequencesList = choosenSequenceList.Items.Cast<string>().ToList();
            test = new Task(() => { testManager.CreateTest(choosenSequencesList); } );

            if (choosenSequencesList != null)
            {
                test.Start();
            }
            else
            {
                MessageBox.Show("Please select at least one step action to execute", "Unable to start test", MessageBoxButtons.OK);
            }


        }

        public void EndTest()
        {
            ChangeEnabled(true);
        }
        #endregion

        #region TestRelatedEventActions

        public void OnStepSucceeded(object sender, TestEventArgs e)
        {
            if(choosenSequenceStatusCheckedList.InvokeRequired)
            {
                TestEventArgs arg = new TestEventArgs() { argument = e.argument };
                choosenSequenceStatusCheckedList.Invoke(new TestManager.StepSucceededEventHandler(OnStepSucceeded), sender, e);
            }
            else
            {
                for(int i = 0; i < choosenSequenceStatusCheckedList.Items.Count; i++)
                {
                    if (choosenSequenceStatusCheckedList.Items[i].ToString() == e.argument && !choosenSequenceStatusCheckedList.GetItemChecked(i))
                    {
                        choosenSequenceStatusCheckedList.SetItemChecked(i, true);
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

        public void OnAppNotInstalled(object sender, TestEventArgs e)
        {
            MessageBox.Show("Selected application :\"" + e.argument + "\" is not installed on this device", "Test Ended", MessageBoxButtons.OK);
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
                consoleOutput.Invoke(new TestManager.LogReadEventHandler(OnLogRead), sender, e);
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

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            choosenSequenceStatusCheckedList.Items.Clear();

            foreach (var itm in choosenSequenceList.Items)
            {
                choosenSequenceStatusCheckedList.Items.Add(itm);
            }
        }

        private void consoleOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (isUpdatingConsoleOutputAllowed)
            {
                isUpdatingConsoleOutputAllowed = false;
            }
            else if(!isUpdatingConsoleOutputAllowed)
            {
                isUpdatingConsoleOutputAllowed = true;

                foreach(string log in consoleOutputBuffer)
                {
                    UpdateConsoleOutput(log);
                }

                consoleOutputBuffer.Clear();                
            }
        }

    }
}
