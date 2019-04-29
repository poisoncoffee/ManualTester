using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{

    public class TestStep
    {
        public enum StepType
        {
            Tap,
            Wait,
            Command,
        }

        public TestDatabase.TestAction actionIfSucceeded;   //What to do if step succeeded
        public TestDatabase.TestAction actionIfFailed;      //What to do if confirmation is absent
        public StepType testStepType;

        public string testStepID;
        public string argument;                             //For now, useful only for steps of type Command - Provide command here
        public List<string> conditionLog;
        public List<string> confirmationLog;

        //Time in miliseconds. If no confirmation arrived after this time, step is recognized as failed. Test Logic checks every 100+ ms 
        public int terminationTime;

        public int posY;
        public int posX;

    }
}
