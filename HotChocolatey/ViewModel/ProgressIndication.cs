using System;
using System.Windows.Threading;

namespace HotChocolatey.ViewModel
{
    public class ProgressIndication : IDisposable
    {
        private readonly Dispatcher dispatcher;
        private readonly Action stopAction;

        public ProgressIndication(Dispatcher dispatcher, Action startAction, Action stopAction)
        {
            this.dispatcher = dispatcher;
            this.stopAction = stopAction;
            dispatcher.Invoke(startAction);
        }

        public void Dispose()
        {
            dispatcher.Invoke(stopAction);
        }
    }
}
