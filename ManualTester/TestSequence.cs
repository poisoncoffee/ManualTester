using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public class TestSequence : IDefinable
    {
        public string sequenceID;
        public List<string> sequenceElements; //List of "children" IDs

        public string GetID()
        {
            return sequenceID;
        }
    }
}
