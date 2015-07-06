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
        public List<ChocoItem> MarkedForUpgrade => Items.Where(t => t.IsMarkedForUpgrade).ToList();
        public List<ChocoItem> MarkedForUninstall => Items.Where(t => t.IsMarkedForUninstall).ToList();

        private readonly Dictionary<string, ChocoItem> packages = new Dictionary<string, ChocoItem>();
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
