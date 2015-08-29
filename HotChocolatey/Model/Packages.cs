using HotChocolatey.Utility;
using HotChocolatey.ViewModel;
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
            Items.Add(item);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
