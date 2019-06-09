using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class TestSequence : TestDefinition
    {
        public List<TestDefinition> elements; //List of "children" IDs

        public override List<TestStep> Flatify()
        {
            List<TestStep> result = new List<TestStep>();
            foreach(TestDefinition element in elements)
            {
                result.AddRange(element.Flatify());
            }
            return result;
        }
    }
}
