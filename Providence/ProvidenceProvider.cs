using System;
using System.Collections.Generic;
using System.Linq;

namespace Providence
{
    public class ProvidenceProvider : ISuggestionProvider
    {
        private readonly IEnumerable<ProvidenceService> _services;

        public ProvidenceProvider(IEnumerable<ProvidenceService> services)
        {
            _services = services;
        }

        public IEnumerable<ISuggestable> GetSuggestions(string text, int maxCount)
        {
            ProvidenceService prefixedService = _services.FirstOrDefault(service => text.TrimStart(' ').ToLowerInvariant().StartsWith(service.Prefix.ToLowerInvariant() + " ", StringComparison.Ordinal));
            if (prefixedService != null)
            {
                string substring = text.TrimStart(' ').Substring(prefixedService.Prefix.Length).Trim();
                return GetSuggestionsFromService(prefixedService, substring, maxCount);
            }
            
            return _services.Where(service => service.Prefix.Contains(text.Trim())).Concat(_services.SelectMany(service => GetSuggestionsFromService(service, text.Trim(), maxCount)).Take(maxCount).ToArray());
        }

        private IEnumerable<ISuggestable> GetSuggestionsFromService(ProvidenceService service, string searchText, int maxCount)
        {
            IEnumerable<ISuggestable> commands = service.GetSuggestions(searchText, maxCount).ToArray();

            foreach (ProvidenceCommand command in commands.OfType<ProvidenceCommand>())
                command.Service = service;

            return commands;
        }
    }
}