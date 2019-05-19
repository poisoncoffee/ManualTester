using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public interface ILogcatOperator
    {
        Logcat BeginLogcat(string processPID, string packagename);

        Logcat UpdateLogcat(Logcat logcat, int offset);

        //TODO - Not Implemented Yet
        void EndLogcat(Logcat logcat);


    }


}
