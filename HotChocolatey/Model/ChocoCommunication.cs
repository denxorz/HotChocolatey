using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace HotChocolatey.Model
{
    public static class ChocoCommunication
    {
        private static Dispatcher dispatcher;
        public static ObservableCollection<string> Communication { get; } = new ObservableCollection<string>();

        public static void SetDispatcher(Dispatcher newDispatcher)
        {
            dispatcher = newDispatcher;
        }

        public static void Log(string data, CommunicationDirection direction)
        {
            dispatcher.Invoke(() =>
            {
                Communication.Add($"{(direction == CommunicationDirection.ToChoco ? ">> " : string.Empty)}{data}");

                if (Communication.Count > 5100)
                {
                    while (Communication.Count > 5000)
                    {
                        Communication.RemoveAt(0);
                    }
                }
            });
        }
    }
}
