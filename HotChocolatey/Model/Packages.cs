using HotChocolatey.Utility;
using HotChocolatey.ViewModel;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class Packages
    {
        public ObservableCollectionEx<ChocoItem> Items { get; } = new ObservableCollectionEx<ChocoItem>();

        private IPackageList packageList;
        private ChocolateyController controller;

        public Packages(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public async Task GetMore()
        {
            if (packageList == null)
            {
                await ApplyPackageList(new AllPackageList(controller));
            }

            if (packageList.HasMore)
            {
                (await packageList.GetMore(20)).ToList().ForEach(Add);
            }
        }

        public async Task ApplySearch(string searchText)
        {
            await packageList.ApplySearch(searchText);
            Clear();
            await GetMore();
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
