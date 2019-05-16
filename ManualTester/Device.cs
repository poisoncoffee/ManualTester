using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class Device
    {
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
    }
}
