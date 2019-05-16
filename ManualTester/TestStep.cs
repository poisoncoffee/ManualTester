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

        public TestDatabase.TestAction actionIfSucceeded;   //[TODO] What to do if step succeeded
        public TestDatabase.TestAction actionIfFailed;      //What to do if confirmation is absent
        public StepType testStepType;                       //[TODO] What is the type of step (ex. Tap, Wait, Command)

        public string testStepID;                           //Name of step. Needs to be unique.
        public string argument;                             //[TODO] For now, useful only for steps of type Command - Provide command here
        public List<string> conditionLog;                   //[TODO] Step will be executed only if at least one of the conditionLogs is present. If the condition is absent, the step ends with success.
        public List<string> confirmationLog;                //Step ends with a success if at least one of the confirmationLog is present. If the confirmation is absent, the stem ends with fail.

        public int waitTime;                               //[TODO] Time in miliseconds. Amount of time the program will wait before executing step.
        public int terminationTime;                         //Time in miliseconds. If no confirmation arrived after this time, step is recognized as failed. Test Logic checks every 500+ ms

        public int posY;                                    //Y position of tap (needs to be provided only for steps of type Tap) [TODO] Support for 1. Different resolutions 2. Different aspect ratios 3. Safe Area sesnivity
        public int posX;                                    //X position of tap (needs to be provided only for steps of type Tap) [TODO] 1. Different resolutions 2. Different aspect ratios 3. Safe Area sesnivity

    }
}
