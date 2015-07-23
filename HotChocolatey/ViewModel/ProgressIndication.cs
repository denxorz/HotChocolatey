using System;

namespace HotChocolatey.ViewModel
{
    public class ProgressIndication : IDisposable
    {
        public interface IProgressIndicator
        {
            bool IsInProgress { set; }
        }

        // TODO: should this include a static stack, so it can be used inside itself?

        private IProgressIndicator indicator;

        public ProgressIndication(IProgressIndicator indicator)
        {
            this.indicator = indicator;
            this.indicator.IsInProgress = true;
        }

        public void Dispose()
        {
            indicator.IsInProgress = false;
        }
    }
}
