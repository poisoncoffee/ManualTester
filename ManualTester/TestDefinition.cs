using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    public abstract class TestDefinition
    {
        public string id;

        public string GetID()
        {
            return id;
        }

        public abstract List<TestStep> Flatify();
    }
}
