using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class UpgradablePackageList : IPackageList
    {
        private readonly ChocolateyController controller;

        private List<ChocoItem> packages = new List<ChocoItem>();
        private int skipped;
        private string searchFor;

        public bool HasMore => packages.Count > skipped;

        public UpgradablePackageList(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public async Task Refresh()
        {
            skipped = 0;
            packages = (await controller.InstalledPackages.GetPackages()).Where(t => t.IsUpgradable).ToList();
        }

        public async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var searchedPackages = string.IsNullOrWhiteSpace(searchFor) ? packages : packages.Where(p => p.Title.Contains(searchFor));
            var tmp = searchedPackages.Skip(skipped).Take(numberOfItems);
            skipped += numberOfItems;
            return tmp;
        }

        public async Task ApplySearch(string searchFor)
        {
            this.searchFor = searchFor;
            skipped = 0;
        }
    }
}
