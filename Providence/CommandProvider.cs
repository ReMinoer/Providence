using System.Collections.Generic;
using System.Linq;

namespace Providence
{
    public class CommandProvider : ISuggestionProvider<IProvidenceCommand>
    {
        private readonly CommandRegistry _commandRegistry;
        public string SearchText { get; set; }

        public CommandProvider(CommandRegistry commandRegistry)
        {
            _commandRegistry = commandRegistry;
        }

        public IProvidenceCommand[] GetSuggestions(int maxCount)
        {
            if (string.IsNullOrEmpty(SearchText))
                return new IProvidenceCommand[0];

            var result = new List<IProvidenceCommand>();

            string[] keywords = SearchText.Trim().Split(' ');

            if (keywords.Length == 1)
            {
                char[] initials = keywords[0].ToLower().ToCharArray();

                foreach (IProvidenceCommand command in _commandRegistry)
                    result.Add(command);

                for (int i = 0; i < initials.Length; i++)
                {
                    var toRemove = new List<IProvidenceCommand>();
                    foreach (IProvidenceCommand command in result)
                        if (command.Initials[i] != initials[i])
                            toRemove.Add(command);

                    foreach (IProvidenceCommand command in toRemove)
                        result.Remove(command);
                }
            }

            foreach (IProvidenceCommand command in _commandRegistry)
            {
                if (command.DisplayName.ToLower().Contains(SearchText.ToLower()) && !result.Contains(command))
                {
                    result.Add(command);

                    if (result.Count >= maxCount)
                        break;
                }
            }

            return result.ToArray();
        }
    }
}