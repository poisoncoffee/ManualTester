using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class TestSet : IDefinable
    {
        public string sequenceID;
        public List<IDefinable> sequenceElements; //List of "children" IDs

        public List<TestStep> Flatify()
        {
            List<TestStep> result = new List<TestStep>();
            foreach(IDefinable element in sequenceElements)
            {
                result.AddRange(element.Flatify());
            }
            return result;
        }

        public string GetID()
        {
            return sequenceID;
        }
    }
}
