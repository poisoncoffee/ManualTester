using System.Collections.Generic;

namespace WindowsFormsApp1
{
    public interface IDefinable
    {
        string GetID();

        List<TestStep> Flatify();
    }
}
