using HotChocolatey.UI;
using HotChocolatey.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace HotChocolatey.Logic
{
    public class Packages
    {
        public ObservableCollectionEx<ChocoItem> Items { get; } = new ObservableCollectionEx<ChocoItem>();

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
            AddSorted(item);
        }

        public void Clear()
        {
            Items.Clear();
        }

        /// <summary>
        /// Based on http://stackoverflow.com/a/16839559/2471080
        /// </summary>
        public void AddSorted(ChocoItem item)
        {
            var comparer = Comparer< ChocoItem>.Create((x, y) => x.Title.CompareTo(y.Title));

            int i = 0;
            while (i < Items.Count && comparer.Compare(Items[i], item) < 0)
                i++;

            Items.Insert(i, item);
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
