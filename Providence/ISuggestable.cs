using System;

namespace Providence
{
    public interface ISuggestable
    {
        string DisplayName { get; }
        string Category { get; }
        string IconSource { get; }
        bool HideAfterRun { get; }
        void Run();
        event Action<Progress> Progressed;
    }
}