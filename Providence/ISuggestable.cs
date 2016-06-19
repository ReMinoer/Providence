namespace Providence
{
    public interface ISuggestable
    {
        string Name { get; }
        string Category { get; }
        string IconSource { get; }
        char[] Initials { get; }
    }
}