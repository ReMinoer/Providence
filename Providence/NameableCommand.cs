using System.Linq;

namespace Providence
{
    public abstract class NameableCommand : ProvidenceCommand, INameableSuggestable
    {
        private string _displayName;
        public char[] Initials { get; private set; }

        public override sealed string DisplayName
        {
            get { return _displayName; }
            protected set
            {
                _displayName = value;
                Initials = DisplayName.Trim().ToLowerInvariant().Split(' ').Select(x => x.First()).ToArray();
            }
        }
    }
}