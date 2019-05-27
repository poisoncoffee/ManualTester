namespace WindowsFormsApp1
{
    public class Device
    {
        public Device()
        {
            //Temporary: For now, only ADB is supported.
            ADBWrapper adbWrapper = new ADBWrapper();
            deviceModel = adbWrapper;
            logcatOperator = adbWrapper;
        }

        public enum Status
        {
            Ready,
            Offline,
            Unauthorized
        }

        public string serial;
        public Status status;
        public int resolutionX = 0;
        public int resolutionY = 0;
        public IDeviceModel deviceModel; 
        public ILogcatOperator logcatOperator;
    }
}
