using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace HotChocolatey.Utility
{
    /// <summary>
    /// http://codeblog.vurdalakov.net/2013/06/fast-addrange-method-for-observablecollection.html
    /// </summary>
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        public void AddRange(IEnumerable<T> list)
        {
            foreach (T item in list)
            {
                Items.Add(item);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void ClearAndAddRange(IEnumerable<T> list)
        {
            Items.Clear();
            AddRange(list);
        }
    }
}
