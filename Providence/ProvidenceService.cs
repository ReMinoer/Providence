using System;
using System.Collections.Generic;

namespace Providence
{
    public abstract class ProvidenceService : ISuggestable
    {
        public bool Enabled { get; internal set; }
        public string DisplayName { get; protected set; }
        public string IconSource { get; protected set; }
        public string Prefix { get; protected set; }
        public string Category => "Service";
        public bool HideAfterRun => false;
        public event Action<Progress> Progressed;
        public event Action<string> InsertPrefixRequested;

        protected ProvidenceService()
        {
            Enabled = true;
        }

        public virtual void Initialize()
        {
        }

        public abstract IEnumerable<ISuggestable> GetSuggestions(string commandLine, int countMax);

        public void Run()
        {
            InsertPrefixRequested?.Invoke(Prefix);
        }
    }
}