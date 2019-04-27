using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public enum StepType
    {
        Tap,
        Wait,
        Command,
    }

    public class TestStep
    {
        public TestDatabase.TestAction actionIfSucceeded;  //what to do if step succeeded
        public TestDatabase.TestAction actionIfFailed;   //what to do if condition or confirmation is absent

        public string testStepType;

        public string testStepID;
        public string conditionLog;
        public string confirmationLog;

        public int posY;
        public int posX;

    }
}
