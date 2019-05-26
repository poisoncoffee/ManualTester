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
            Back,
            Wait,
            ExecuteScript
        }

        public TestDatabase.TestAction actionIfSucceeded = TestDatabase.TestAction.Stop;    //[TODO] What to do if step succeeded
        public TestDatabase.TestAction actionIfFailed = TestDatabase.TestAction.Stop;       //What to do if confirmation is absent
        public StepType testStepType;                                                       //TODO more types

        public string testStepID;                                                           //Name of step. Needs to be unique.
        public List<string> conditionLog;                                                   //[TODO] Step will be executed only if at least one of the conditionLogs is present. If the condition is absent, the step ends with success.
        public List<string> confirmationLog;                                                //Step ends with a success if at least one of the confirmationLog is present. If the confirmation is absent, the stem ends with fail.

        public int waitTime = 5000;                                                         //Time in miliseconds. Amount of time the program will wait before executing step.
        public int terminationTime = 0;                                                     //Time in miliseconds. If no confirmation arrived after this time, step is recognized as failed. Test Logic checks every 500+ ms

        public double posY = 0;                                                             //Range 0-1 [TODO] Support for 1. Different aspect ratios 2. Safe Area sesnivity
        public double posX = 0;                                                             //Range 0-1 [TODO] Support for 1. Different aspect ratios 2. Safe Area sesnivity

        public string scriptName;                                                           //Name script file (script should be placed in the same directory as other apps Definitions)

    }
}
