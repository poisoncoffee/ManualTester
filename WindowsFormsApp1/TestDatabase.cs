using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public class TestDatabase
    {
        private static TestDatabase instance = null;
        private static readonly object padlock = new object();

        TestDatabase()
        {
        }

        public static TestDatabase Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TestDatabase();
                    }
                    return instance;
                }
            }
        }

        public enum TestAction
        {
            Next,
            Stop,
            Retry
        }

        public string GetMainActivityName(string packagename)
        {
            //not implemented yet
            return "com.prime31.UnityPlayerNativeActivity";
        }

        public List<TestStep> LoadTestStepDefinitions(string packagename)
        {
            string testStepDefinitionsFilePath = packagename + "/Definitions/TestStepDefinitions.json";

            string rawJson = "";
            rawJson = System.IO.File.ReadAllText(testStepDefinitionsFilePath);

            StepDefinitions testDefinitions = new StepDefinitions();
            testDefinitions = JsonConvert.DeserializeObject<TestStep>(rawJson);

            return testDefinitions.TestStepDefinitions;
        }

        public List<TestSequence> LoadTestSequenceDefinitions(string packagename)
        {
            string testStepDefinitionsFilePath = packagename + "/Definitions/TestSequenceDefinitions.json";

            //try opening the file
            string rawJson = "";
            rawJson = System.IO.File.ReadAllText(testStepDefinitionsFilePath);
            SequenceDefinitions testSequenceDefinitions = new SequenceDefinitions();

            testSequenceDefinitions = JsonConvert.DeserializeObject<SequenceDefinitions>(rawJson);
            foreach (TestSequence tsq in testSequenceDefinitions.TestSequenceDefinitions)
            {

            }
            return testSequenceDefinitions.TestSequenceDefinitions;
        }

    }
}
