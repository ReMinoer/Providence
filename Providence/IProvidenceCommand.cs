using System;

namespace Providence
{
    public struct Progress
    {
        public double Value { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public string Message { get; set; }
        public bool IsIndeterminate { get; set; }

        static public Progress Default => new Progress { Maximum = 1 };
    }

    public interface IProvidenceCommand : ISuggestable
    {
        event Action<Progress> Progressed;
        void Run();
    }
}