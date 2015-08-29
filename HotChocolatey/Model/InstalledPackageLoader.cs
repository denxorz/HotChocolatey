using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class InstalledPackageLoader
    {
        private Task<List<ChocoItem>> loadingTask;

        public InstalledPackageLoader(ChocolateyController controller)
        {
            loadingTask = controller.GetInstalled();
        }

        public async Task<List<ChocoItem>> GetPackages()
        {
            return await loadingTask;
        }
    }
}
