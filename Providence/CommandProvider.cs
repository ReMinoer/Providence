using System.Collections.Generic;

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
            var result = new List<IProvidenceCommand>();

            foreach (IProvidenceCommand command in _commandRegistry)
                if (command.DisplayName.ToLower().Contains(SearchText.ToLower()))
                    result.Add(command);

            return result.ToArray();
        }
    }
}