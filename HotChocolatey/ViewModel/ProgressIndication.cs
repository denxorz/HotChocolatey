using System;

namespace HotChocolatey.ViewModel
{
    public class ProgressIndication : IDisposable
    {
        private readonly Action stopAction;

        public ProgressIndication(Action startAction, Action stopAction)
        {
            this.stopAction = stopAction;
            startAction();
        }

        public void Dispose()
        {
            stopAction();
        }
    }
}
