using System.Windows;

namespace Providence
{
    public class TestCommand : IProvidenceCommand
    {
        public string DisplayName { get; }

        public TestCommand(string displayName)
        {
            DisplayName = displayName;
        }

        public void Run()
        {
            Window window = Application.Current.Windows[0];
            if (window != null)
                MessageBox.Show(window, DisplayName);
        }
    }
}