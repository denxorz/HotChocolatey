using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class InstalledPackageList : PackageListBase
    {
        private readonly ChocolateyController controller;

        private List<ChocoItem> packages = new List<ChocoItem>();

        public override bool HasMore => packages.Count > skipped;

        public InstalledPackageList(ChocolateyController controller)
        {
            this.controller = controller;
        }

        public override async Task Refresh()
        {
            skipped = 0;
            packages = (await controller.InstalledPackages.GetPackages());
        }

        public override async Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems)
        {
            var searchedPackages = string.IsNullOrWhiteSpace(searchFor) ? packages : packages.Where(p => PackageSearchComparer(p, searchFor));
            var tmp = searchedPackages.Skip(skipped).Take(numberOfItems);
            skipped += numberOfItems;
            return tmp;
        }
    }
}
