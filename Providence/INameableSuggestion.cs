namespace Providence
{
    public interface INameableSuggestable : ISuggestable
    {
        char[] Initials { get; }
    }
}