using log4net.Appender;
using log4net.Core;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using PropertyChanged;

namespace HotChocolatey.ViewModel
{
    [ImplementPropertyChanged]
    public class Diagnostics : IAppender
    {
        public ObservableCollection<string> Logging { get; } = new ObservableCollection<string>();
        private readonly Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        string IAppender.Name { get; set; }

        void IAppender.Close()
        {
            // No need to close anything
        }

        void IAppender.DoAppend(LoggingEvent loggingEvent)
        {
            dispatcher.Invoke(() =>
            {
                Logging.Add(loggingEvent.MessageObject.ToString());

                if (Logging.Count > 5100)
                {
                    while (Logging.Count > 5000)
                    {
                        Logging.RemoveAt(0);
                    }
                }
            });
        }
    }
}
