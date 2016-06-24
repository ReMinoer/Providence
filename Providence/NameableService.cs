using System.Collections.Generic;
using System.Linq;

namespace Providence
{
    public abstract class NameableService : ProvidenceService
    {
        private NameableCommand[] _commands;

        public override void Initialize()
        {
            _commands = GetNameables().ToArray();
        }

        public abstract IEnumerable<NameableCommand> GetNameables();

        public override sealed IEnumerable<ISuggestable> GetSuggestions(string commandLine, int countMax)
        {
            if (string.IsNullOrEmpty(commandLine))
                return _commands.Take(countMax).ToArray();

            var result = new List<NameableCommand>();

            string[] keywords = commandLine.Trim().Split(' ');

            if (keywords.Length == 1)
            {
                char[] initials = keywords[0].ToLower().ToCharArray();

                result.AddRange(_commands);

                for (int i = 0; i < initials.Length; i++)
                {
                    var toRemove = new List<NameableCommand>();

                    foreach (NameableCommand command in result)
                        if (i >= command.Initials.Length || command.Initials[i] != initials[i])
                            toRemove.Add(command);

                    foreach (NameableCommand command in toRemove)
                        result.Remove(command);
                }
            }

            foreach (NameableCommand command in _commands)
            {
                if (command.DisplayName.ToLower().Contains(commandLine.ToLower()) && !result.Contains(command))
                {
                    result.Add(command);

                    if (result.Count >= countMax)
                        break;
                }
            }

            return result.ToArray();
        }
    }
}