using System;
using System.Collections.Generic;
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
        public bool forceTest = false;     // if set to true, launches test despite the fact DeviceModel failed to launch app. Used with assumption that end user launched the app manually.
        
        public TestManager()
        {

        }

        public void CreateTest(string packagename)
        {

            if(DeviceModel.IsDeviceReady())
            {
                if(DeviceModel.IsAppInstalled(packagename))
                {

                    if(DeviceModel.LaunchApp(packagename) || forceTest )
                    {

                    } 
                    else
                    {
                        OnAppLaunchFailed(packagename);
                    }
                }
                else
                {
                    OnAppNotInstalled(packagename);
                }

            }
            else
            {
                OnDeviceNotConnected(packagename);
                OnStepSucceeded();
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

        public delegate void AppNotInstalledEventHandler(object sender, TestEventArgs e);
        public event AppNotInstalledEventHandler AppNotInstalled;
        protected virtual void OnAppNotInstalled(string packagename)
        {
            if (AppNotInstalled != null)
            {
                AppNotInstalled(this, new TestEventArgs() { argument = packagename });
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
