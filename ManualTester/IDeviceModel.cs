using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public interface IDeviceModel
    {
        bool LaunchApp(string packageName);

        void InputTap(int x, int y);

        void InputBack();

        bool IsDeviceReady();

        bool IsAppInstalled(string packageName);

        string GetProcessPID(string packageName);

        List<Device> GetConnectedDevices();

        List<Device> GetReadyDevicesFullInfo();

        Device GetDevicesResolution(Device device);


    }
}
