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
        private List<TestSequence> choosenSequencesList = new List<TestSequence>();


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

        public void StartTest(bool force)
        {

            ChangeEnabled(false);

            string packagename = comboBoxWithAvailableApps.Text;
            var testManager = new TestManager(packagename);

            //subscribe to all events
            testManager.TestEnded += OnTestEnded;
            testManager.StepSucceeded += OnStepSucceeded;
            testManager.DeviceNotConnected += OnDeviceNotConnected;
            testManager.AppLaunchFailed += OnAppLaunchFailed;

            //force set to true launches test despite fact that app could not be opened via adb - assumes the user launched the app manually. See OnAppLaunchFailed event
            if (force)
                testManager.forceTest = true;


            List<string> choosenSequences = choosenSequenceList.Items.Cast<string>().ToList();

            Task test = new Task(() => { testManager.CreateTest(choosenSequences); } );

            if (choosenSequenceStatusCheckedList != null)
            {
                //run test manager with appropiate packagename
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

        public void OnStepSucceeded(object sender, EventArgs e)
        {
            choosenSequenceStatusCheckedList.Items.Add("Success");
        }

        public void OnTestEnded(object sender, TestEventArgs e)
        {
            MessageBox.Show(e.argument, "Test Ended", MessageBoxButtons.OK);
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

        #endregion

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            choosenSequenceStatusCheckedList.Items.Clear();

            foreach (var itm in choosenSequenceList.Items)
            {
                choosenSequenceStatusCheckedList.Items.Add(itm);
            }
        }
    }
}
