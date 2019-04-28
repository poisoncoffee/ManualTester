using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public class TestStep
    {
        public TestDatabase.TestAction actionIfSucceeded;   //What to do if step succeeded
        public TestDatabase.TestAction actionIfFailed;      //What to do if condition or confirmation is absent

        public string testStepID;
        public List<string> conditionLog;
        public List<string> confirmationLog;

        //Time in miliseconds. If no confirmation arrived after this time, step is recognized as failed. Lowest value is 50 ms
        public int terminationTime;

        public int posY;
        public int posX;

    }
}
