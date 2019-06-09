using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class TestModel
    {
        public IDeviceModel currentPlatform { get; private set; }

        //TODO. Temporary solution - only one platform is supported right now
        public void SetCurrentPlatform()
        {
            currentPlatform = new ADBWrapper();
        }

        public bool InitializeDefinitions()
        {
            if (Settings.chosenApp == null)
                return false;

            TestDatabase.LoadDefinitions();

            return true;
        }

        public List<string> GetAvailableSequences()
        {
            List<string> sequences = TestDatabase.ConvertIDefinablesToStrings(TestDatabase.sequenceDefinitions);
            return sequences;
        }






    }
}
