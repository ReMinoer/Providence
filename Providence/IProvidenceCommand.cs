namespace Providence
{
    public interface IProvidenceCommand
    {
        string DisplayName { get; }
        char[] Initials { get; }
        void Run();
    }
}