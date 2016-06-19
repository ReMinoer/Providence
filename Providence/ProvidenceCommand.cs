using System;
using System.ComponentModel;
using System.Linq;
using Providence.Attributes;

namespace Providence
{
    public abstract class ProvidenceCommand : IProvidenceCommand
    {
        public string Name { get; }
        public string Category { get; }
        public string IconSource { get; }
        public char[] Initials { get; }

        public event Action<Progress> Progressed;

        protected ProvidenceCommand()
        {
            object[] attributes = GetType().GetCustomAttributes(true);

            Name = attributes.OfType<DisplayNameAttribute>().FirstOrDefault()?.DisplayName ?? GetType().Name;
            Category = attributes.OfType<CategoryAttribute>().FirstOrDefault()?.Category;
            IconSource = attributes.OfType<IconAttribute>().FirstOrDefault()?.IconSource;
            Initials = Name?.Trim().ToLower().Split(' ').Select(x => x.First()).ToArray();
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