using System;

namespace HotChocolatey
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
            Log.Info("ProgressIndication.ctor");

            this.indicator = indicator;
            this.indicator.IsInProgress = true;
        }

        void IDisposable.Dispose()
        {
            indicator.IsInProgress = false;
            Log.Info("ProgressIndication.Dispose");
        }
    }
}
