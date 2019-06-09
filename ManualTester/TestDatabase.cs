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

        public static List<string> ConvertIDefinablesToStrings<T>(List<T> iDefinables) where T : IDefinable, new()
        {
            List<string> stringIDefinables = iDefinables.ConvertAll(new Converter<T, string>(IDefinableToString));
            return stringIDefinables;
        }

        private static string IDefinableToString<T>(T definable) where T: IDefinable
        {
            return definable.GetID();
        }

        public static List<T> ConvertStringsToIDefinables<T>(List<string> stringIDefinables, T targetType) where T : IDefinable, new()
        {
            List<IDefinable> iDefinables = stringIDefinables.ConvertAll(new Converter<string, IDefinable>(StringToIDefinable));
            List<T> targets = iDefinables.OfType<T>().ToList();

            return targets;
        }

        private static IDefinable StringToIDefinable(string id)
        {
            //first, looks for TestStep - if no TestStep found, looks for TestSequence
            IDefinable result = stepDefinitions.Find(d => d.GetID() == id);
            if (result == null)
                result = sequenceDefinitions.Find(d => d.GetID() == id);

            return result;
        }

        public static List<TestStep> UnfoldSequences(List<TestSequence> sequences)
        {
            List<TestStep> stepList = new List<TestStep>();

            foreach(TestSequence sequence in sequences)
            {
                stepList.AddRange(UnfoldSequence(sequence));
            }

            return stepList;
        }

        private static List<TestStep> UnfoldSequence(TestSequence sequence)
        {
            List<TestStep> stepList = new List<TestStep>();
            TestStep entry = new TestStep();
            foreach (string child in sequence.sequenceElements)
            {
                IDefinable result = StringToIDefinable(child);
                if (!(result.GetType() == entry.GetType()))
                    UnfoldSequence((TestSequence)result);

                stepList.Add((TestStep)result);
            }

            return stepList;
        }

    }
}
