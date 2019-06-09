using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WindowsFormsApp1
{
    public class TestDatabase
    {
        private static Dictionary<string, IDefinable> definitions = new Dictionary<string, IDefinable>();

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
            List<TestStep> stepDefinitions = DeserializeDefinitions<TestStep>(Settings.stepDefinitionsFilePath);
            foreach(TestStep step in stepDefinitions)
            {
                definitions.Add(step.GetID(), step);
            }

            List<RawTestSequence> rawSequenceDefinitions = DeserializeDefinitions<RawTestSequence>(Settings.sequenceDefinitionsFilePath);
            foreach(RawTestSequence rawSequence in rawSequenceDefinitions)
            {
                TestSet sequence = new TestSet();
                sequence.sequenceID = rawSequence.GetID();

                sequence.sequenceElements = definitions.Values.ToList().Where(p => rawSequence.sequenceElements.Any(l => p.GetID() == l)).ToList();

                definitions.Add(sequence.GetID(), sequence);
            }
        }

        public static List<string> ConvertIDefinablesToStrings(List<IDefinable> iDefinables)
        {
            List<string> stringIDefinables = iDefinables.ConvertAll(new Converter<IDefinable, string>(IDefinableToString));
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
            var result = definitions.Where(p => p.Key == id).ToList();
            if (result.Count == 1)
                return result[0].Value;
            else if(result.Count == 0)
                throw new ArgumentException("Element of ID: " + id + " not found.");
            else
                throw new ArgumentException("Definition ID: " + id + " is not unique! Found " + result.Count + " occurencies of " + id + "!");
        }

        public static List<IDefinable> GetLoadedDefinitions()
        {
            return definitions.Values.ToList();
        }

    }
}
