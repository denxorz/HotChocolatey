using HotChocolatey.Utility;
using HotChocolatey.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HotChocolatey.Model
{
    public class Packages
    {
        public ObservableCollectionEx<ChocoItem> Items { get; } = new ObservableCollectionEx<ChocoItem>();

        private readonly ICollectionView view;

        private IPackageList packageList;
        private ChocolateyController controller;
        private ProgressIndication.IProgressIndicator progressIndicator;

        public Packages(ChocolateyController controller, ProgressIndication.IProgressIndicator progressIndicator)
        {
            this.controller = controller;
            this.progressIndicator = progressIndicator;

            view = CollectionViewSource.GetDefaultView(Items);
        }

        public async Task GetMore()
        {
            if (packageList == null)
            {
                await ApplyPackageList(new AllPackageList(controller, progressIndicator));
            }

            if (packageList.HasMore)
            {
                (await packageList.GetMore(20)).ToList().ForEach(Add);
            }
        }

        public async Task ApplyFilter(IFilter filter)
        {
            await ApplyPackageList(filter.CreatePackageList());
        }

        private async Task ApplyPackageList(IPackageList list)
        {
            Clear();
            packageList = list;
            await packageList.Refresh();
        }

        private void Add(ChocoItem item)
        {
            Items.Remove(Items.FirstOrDefault(t => t.Package.Id == item.Package.Id));
            //AddSorted(item);
            Items.Add(item);
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
