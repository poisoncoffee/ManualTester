using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WindowsFormsApp1
{
    public class TestDatabase
    {
        public static List<TestStep> stepDefinitions { get; private set; } = new List<TestStep>();
        public static List<TestSequence> sequenceDefinitions { get; private set; } = new List<TestSequence>();

        public enum TestAction
        {
            Next,
            Stop,
            Back,
            BackAndRetry,
            Retry
        }

        private static List<T> DeserializeDefinitions<T>(string path) where T : class, new()
        {
            string rawJson = System.IO.File.ReadAllText(path);
            List<T> Definitions = JsonConvert.DeserializeObject<List<T>>(rawJson);
            return Definitions;
        }

        public static void LoadDefinitions()
        {
            stepDefinitions = DeserializeDefinitions<TestStep>(Settings.stepDefinitionsFilePath);
            sequenceDefinitions = DeserializeDefinitions<TestSequence>(Settings.sequenceDefinitionsFilePath);
        }

        public static List<string> GetSequenceDefinitionsAsString()
        {
            List<string> sequencesList = TestDatabase.sequenceDefinitions.ConvertAll(new Converter<TestSequence, string>(TestSequenceToString));
            return sequencesList;
        }

        private static string TestSequenceToString(TestSequence sequence)
        {
            return sequence.testSequenceID;
        }


        //Converting choosenSequences (which are strings) to actual TestSequence objects by name
        private static List<TestSequence> ConvertSequencesAsStringToSequences(List<string> choosenSequences)
        {
            List<TestSequence> sequencePlan = new List<TestSequence>();

            foreach (string sequenceName in choosenSequences)
            {
                foreach (TestSequence testSequence in sequenceDefinitions)
                {
                    if (sequenceName == testSequence.testSequenceID)
                    {
                        sequencePlan.Add(testSequence);
                        break;
                    }
                }
            }

            return sequencePlan;
        }

        private static List<TestStep> ConvertSequencesToSteps(List<TestSequence> sequencePlan)
        {
            List<TestStep> stepPlan = new List<TestStep>();
            foreach (TestSequence testSequence in sequencePlan)
            {
                foreach (string testStepOnList in testSequence.StepList)
                {
                    foreach (TestStep testStepDefinition in stepDefinitions)
                    {
                        if (testStepOnList == testStepDefinition.testStepID)
                        {
                            stepPlan.Add(testStepDefinition);
                            break;
                        }
                    }
                }

            }
            return stepPlan;
        }

        public static List<TestStep> ConvertSequencesAsStringToSteps(List<string> choosenSequences)
        {
            List<TestSequence> testSequences = ConvertSequencesAsStringToSequences(choosenSequences);
            List<TestStep> testPlan = ConvertSequencesToSteps(testSequences);
            return testPlan;
        }

        public static List<string> ConvertSequencesAsStringToTestStepsAsString(List<string> choosenSequences)
        {
            List<string> testPlanAsString = new List<string>();
            List<TestStep> testPlan = ConvertSequencesAsStringToSteps(choosenSequences);

            foreach(TestStep ts in testPlan)
            {
                testPlanAsString.Add(ts.testStepID);
            }

            return testPlanAsString;
        }

    }
}
