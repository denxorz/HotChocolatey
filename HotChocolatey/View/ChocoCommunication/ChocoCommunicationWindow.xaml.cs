using System.Collections.Specialized;
using MahApps.Metro.Controls;

namespace HotChocolatey.View.ChocoCommunication
{
    public partial class ChocoCommunicationWindow : MetroWindow
    {
        public ChocoCommunicationWindow()
        {
            InitializeComponent();
            ((INotifyCollectionChanged)loggingListBox.Items).CollectionChanged += OnLoggingListViewCollectionChanged;
            loggingListBox.ScrollIntoView(loggingListBox.Items[loggingListBox.Items.Count - 1]);
        }

        private void OnLoggingListViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                loggingListBox.ScrollIntoView(e.NewItems[0]);
            }
        }
    }
}
