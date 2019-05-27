﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class TestDatabase
    {
        private static TestDatabase instance = null;
        private static readonly object padlock = new object();

        private static SequenceDefinitions sequenceDefinitions = new SequenceDefinitions();
        private static StepDefinitions stepDefinitions = new StepDefinitions();

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
            Back,
            BackAndRetry,
            Retry
        }

        public void LoadTestStepDefinitions()
        {
            if (stepDefinitions.TestStepDefinitions != null)
                stepDefinitions.TestStepDefinitions.Clear();

            string rawJson = "";
            rawJson = System.IO.File.ReadAllText(Settings.stepDefinitionsFilePath);
            stepDefinitions = JsonConvert.DeserializeObject<StepDefinitions>(rawJson);
        }

        public void LoadTestSequenceDefinitions()
        {
            if(sequenceDefinitions.TestSequenceDefinitions != null)
                sequenceDefinitions.TestSequenceDefinitions.Clear();

            string rawJson = "";
            rawJson = System.IO.File.ReadAllText(Settings.sequenceDefinitionsFilePath);
            sequenceDefinitions = JsonConvert.DeserializeObject<SequenceDefinitions>(rawJson);
        }

        public List<string> GetSequenceDefinitionsAsString()
        {
            List<string> sequencesList = new List<string>();

            foreach(TestSequence tsq in sequenceDefinitions.TestSequenceDefinitions)
            {
                sequencesList.Add(tsq.testSequenceID);
            }

            return sequencesList;
        }

        //Converting choosenSequences (which are strings) to actual TestSequence objects by name
        private List<TestSequence> ConvertSequencesAsStringToSequences(List<string> choosenSequences)
        {
            List<TestSequence> sequencePlan = new List<TestSequence>();

            foreach (string sequenceName in choosenSequences)
            {
                foreach (TestSequence testSequence in sequenceDefinitions.TestSequenceDefinitions)
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

        private List<TestStep> ConvertSequencesToSteps(List<TestSequence> sequencePlan)
        {
            List<TestStep> stepPlan = new List<TestStep>();
            foreach (TestSequence testSequence in sequencePlan)
            {
                foreach (string testStepOnList in testSequence.StepList)
                {
                    foreach (TestStep testStepDefinition in stepDefinitions.TestStepDefinitions)
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

        public List<TestStep> ConvertSequencesAsStringToSteps(List<string> choosenSequences)
        {
            List<TestSequence> testSequences = ConvertSequencesAsStringToSequences(choosenSequences);
            List<TestStep> testPlan = ConvertSequencesToSteps(testSequences);
            return testPlan;
        }

        public List<string> ConvertSequencesAsStringToTestStepsAsString(List<string> choosenSequences)
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
