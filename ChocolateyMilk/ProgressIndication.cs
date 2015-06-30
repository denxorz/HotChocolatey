using System;

namespace ChocolateyMilk
{
    public class ProgressIndication : IDisposable
    {
        public interface IProgressIndicator
        {
            bool IsInProgress { set; }
            string StatusText { set; }
        }

        // TODO: should this include a static stack, so it can be used inside itself?

        private IProgressIndicator indicator;

        public ProgressIndication(IProgressIndicator indicator)
        {
            this.indicator = indicator;
            this.indicator.IsInProgress = true;
        }

        void IDisposable.Dispose()
        {
            indicator.IsInProgress = false;
            indicator.StatusText = "Ready";
        }
    }
}
