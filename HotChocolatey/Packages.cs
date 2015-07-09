using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace HotChocolatey
{
    public class Packages
    {
        public ObservableCollection<ChocoItem> Items { get; } = new ObservableCollection<ChocoItem>();

        private readonly ICollectionView view;

        public Packages()
        {
            view = CollectionViewSource.GetDefaultView(Items);
        }

        public void ApplyFilter(IFilter filter)
        {
            view.Filter = filter.Filter;
        }

        public void Add(ChocoItem item)
        {
            Items.Remove(Items.FirstOrDefault(t => t.Package.Id == item.Package.Id));
            Items.Add(item);
            Sort();
        }

        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Based on http://stackoverflow.com/a/1945701/2471080
        /// </summary>
        private void Sort()
        {
            List<ChocoItem> sorted = Items.OrderBy(x => x.Title).ToList();

            int ptr = 0;
            while (ptr < sorted.Count)
            {
                if (!Items[ptr].Equals(sorted[ptr]))
                {
                    ChocoItem t = Items[ptr];
                    Items.RemoveAt(ptr);
                    Items.Insert(sorted.IndexOf(t), t);
                }
                else
                {
                    ptr++;
                }
            }
        }
    }
}
