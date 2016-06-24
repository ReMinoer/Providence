using System.Collections.Generic;

namespace Providence
{
    public interface ISuggestionProvider
    {
        IEnumerable<ISuggestable> GetSuggestions(string text, int maxCount);
    }
}