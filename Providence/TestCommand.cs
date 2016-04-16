using System.Linq;
using System.Windows;

namespace Providence
{
    public class TestCommand : IProvidenceCommand
    {
        public string DisplayName { get; }
        public char[] Initials { get; }

        public TestCommand(string displayName)
        {
            DisplayName = displayName;

            Initials = DisplayName.Trim().ToLower().Split(' ').Select(x => x.First()).ToArray();
        }

        public void Run()
        {
            Window window = Application.Current.Windows[0];
            if (window != null)
                MessageBox.Show(window, DisplayName);
        }
    }
}