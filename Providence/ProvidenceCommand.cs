using System;

namespace Providence
{
    public abstract class ProvidenceCommand : ISuggestable
    {
        public virtual string DisplayName { get; protected set; }
        public string IconSource { get; protected set; }
        public bool HideAfterRun { get; protected set; }
        public ProvidenceService Service { get; internal set; }
        public string Category => Service.DisplayName;
        public event Action<Progress> Progressed;

        protected ProvidenceCommand()
        {
            HideAfterRun = true;
        }

        public abstract void Run();

        protected void ReportProgress(string message = "")
        {
            var args = new Progress
            {
                IsIndeterminate = true,
                Message = message
            };

            Progressed?.Invoke(args);
        }

        protected void ReportProgress(double value, string message = "")
        {
            var args = new Progress
            {
                Value = value,
                Maximum = 1,
                Message = message
            };

            Progressed?.Invoke(args);
        }

        protected void ReportProgress(double value, double maximum, string message = "")
        {
            var args = new Progress
            {
                Value = value,
                Maximum = maximum,
                Message = message
            };

            Progressed?.Invoke(args);
        }

        protected void ReportProgress(double value, double minimum, double maximum, string message = "")
        {
            var args = new Progress
            {
                Value = value,
                Minimum = minimum,
                Maximum = maximum,
                Message = message
            };

            Progressed?.Invoke(args);
        }
    }
}