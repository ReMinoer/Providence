using System.Collections.Generic;

namespace Providence
{
    public class CommandRegistry : List<IProvidenceCommand>
    {
        public CommandRegistry()
        {
            Add(new TestCommand("First test"));
            Add(new TestCommand("Second test"));
            Add(new TestCommand("Third test"));
        }
    }
}