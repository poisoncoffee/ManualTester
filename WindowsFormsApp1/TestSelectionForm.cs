using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;


namespace WindowsFormsApp1
{
    public partial class TestSelectionForm : Form
    {
        public TestSelectionForm()
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
            List<TestSequence> testSequencesList = testDatabase.LoadTestSequenceDefinitions("com.artifexmundi.balefire");

            foreach (TestSequence tsq in testSequencesList)
            {
                sequencesList.Items.Add(tsq.name);
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
             //       c.Enabled = enabled;
                }
            }
        }

        public void StartTest(bool force)
        {

            ChangeEnabled(false);

            var testManager = new TestManager();

            //subscribe to all events
            testManager.TestEnded += OnTestEnded;
            testManager.DeviceNotConnected += OnDeviceNotConnected;
            testManager.AppNotInstalled += OnAppNotInstalled;
            testManager.AppLaunchFailed += OnAppLaunchFailed;

            //force set to true launches test despite fact that app could not be opened via adb - assumes the user launched the app manually. See OnAppLaunchFailed event
            if (force)
                testManager.forceTest = true;

            string packagename = comboBoxWithAvailableApps.Text;
            List<string> choosenSequences = selectedList.Items.Cast<string>().ToList();


            //run test manager with appropiate packagename

            var test = new Task(() => { testManager.CreateTest(packagename); } );
            test.Start();



        }

        public void EndTest()
        {
            ChangeEnabled(true);
        }
        #endregion


        #region TestRelatedEventActions

        public void OnStepSucceeded(object sender, EventArgs e)
        {

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

        private void addSequence_Click(object sender, EventArgs e)
        {
            if (sequencesList.SelectedItem != null)
            {
                selectedList.Items.Add(sequencesList.SelectedItem);
            }
        }

        private void removeSequence_Click(object sender, EventArgs e)
        {
            if (selectedList.SelectedItem != null)
            {
                selectedList.Items.Remove(selectedList.SelectedItem);
            }

        }

        private void moveSequenceUp_Click(object sender, EventArgs e)
        {
            if (selectedList.SelectedItem != null)
            {

                if (selectedList.SelectedIndex != 0)
                {
                    int i = selectedList.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on higher position
                    // 3. overwriting item on higher position with variable
                    var selectedItem = selectedList.Items[i];
                    selectedList.Items[i] = selectedList.Items[i - 1];
                    selectedList.Items[i - 1] = selectedItem;

                    selectedList.SetSelected(i - 1, true);
                }

            }

        }

        private void moveSequenceDown_Click(object sender, EventArgs e)
        {
            if (selectedList.SelectedItem != null)
            {

                if (selectedList.SelectedIndex != selectedList.Items.Count - 1 )
                {
                    int i = selectedList.SelectedIndex;

                    // 1. copying item to another variable
                    // 2. overwriting it with item on lower position
                    // 3. overwriting item on lower position with variable
                    var selectedItem = selectedList.Items[i];
                    selectedList.Items[i] = selectedList.Items[i + 1];
                    selectedList.Items[i + 1] = selectedItem;

                    selectedList.SetSelected(i + 1, true);
                }

            }
        }

    }
}
