using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace ChocolateyMilk
{
    public class Packages
    {
        public ObservableCollection<ChocoItem> Items { get; } = new ObservableCollection<ChocoItem>();
        public List<ChocoItem> MarkedForInstallation => Items.Where(t => t.IsMarkedForInstallation).ToList();

        private readonly Dictionary<string, ChocoItem> packages = new Dictionary<string, ChocoItem>();
        private readonly ICollectionView view;

        public Packages()
        {
            view = CollectionViewSource.GetDefaultView(Items);
        }

        public void ApplyFilter(Predicate<object> filter)
        {
            view.Filter = filter;
        }

        public void Add(ChocoItem item)
        {
            if (packages.ContainsKey(item.Name))
            {
                packages[item.Name].Update(item);
            }
            else
            {
                packages.Add(item.Name, item);
                Items.Add(item);
            }
        }

        public void Clear()
        {
            Items.Clear();
            packages.Clear();
        }
    }
}
