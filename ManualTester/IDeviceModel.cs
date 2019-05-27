using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public interface IDeviceModel
    {

        //TODO: Support for executing on particular device (for multiple devices)

        bool LaunchApp(Device device);

        void InputTap(int x, int y);

        void InputBack();

        bool IsDeviceReady();

        bool IsAppInstalled();

        void ExecuteInShell(string command);

        void ExecuteScript(string scriptPath);

        List<Device> GetConnectedDevices();

        List<Device> GetReadyDevicesFullInfo();

        Device GetDevicesResolution(Device device);


    }
}
