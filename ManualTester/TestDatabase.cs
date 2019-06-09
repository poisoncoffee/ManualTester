using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WindowsFormsApp1
{
    public class TestDatabase
    {
        private static Dictionary<string, TestDefinition> definitions = new Dictionary<string, TestDefinition>();

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
                TestSequence sequence = new TestSequence();
                sequence.id = rawSequence.GetID();

                sequence.elements = definitions.Values.ToList().Where(p => rawSequence.elements.Any(l => p.GetID() == l)).ToList();

                definitions.Add(sequence.GetID(), sequence);
            }
        }

        public static List<string> ConvertTestDefinitionsToStrings(List<TestDefinition> testDefinitions)
        {
            List<string> stringDefinitions = testDefinitions.ConvertAll(new Converter<TestDefinition, string>(TestDefinitionToString));
            return stringDefinitions;
        }

        private static string TestDefinitionToString(TestDefinition definition)
        {
            return definition.GetID();
        }

        public static List<T> ConvertStringsToTestDefinitions<T>(List<string> stringDefinitions, T targetType) where T : TestDefinition, new()
        {
            List<TestDefinition> testDefinitions = stringDefinitions.ConvertAll(new Converter<string, TestDefinition>(StringToTestDefinition));
            List<T> targets = testDefinitions.OfType<T>().ToList();

            return targets;
        }

        private static TestDefinition StringToTestDefinition(string id)
        {
            var result = definitions.Where(p => p.Key == id).ToList();
            if (result.Count == 1)
                return result[0].Value;
            else if(result.Count == 0)
                throw new ArgumentException("Element of ID: " + id + " not found.");
            else
                throw new ArgumentException("Definition ID: " + id + " is not unique! Found " + result.Count + " occurencies of " + id + "!");
        }

        public static List<TestDefinition> GetLoadedDefinitions()
        {
            return definitions.Values.ToList();
        }

    }
}
