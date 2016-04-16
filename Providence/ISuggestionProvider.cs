namespace Providence
{
    public interface ISuggestionProvider<out T>
    {
        T[] GetSuggestions(int maxCount);
    }
}