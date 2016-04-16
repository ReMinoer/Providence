namespace Providence
{
    public interface IProvidenceCommand
    {
        string DisplayName { get; }
        void Run();
    }
}