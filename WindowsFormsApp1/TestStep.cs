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

    public class TestStep : StepDefinitions
    {
        public string testStepType { get; private set; }

        public string testStepID { get; private set; }
        public string conditionLog { get; private set; }
        public string confirmationLog { get; private set; }

        public TestDatabase.TestAction actionIfSucceeded;  //what to do if step succeeded
        public TestDatabase.TestAction actionIfFailed;   //what to do if condition or confirmation is absent

        public double posY { get; private set; }
        public double posX { get; private set; }

    }
}
