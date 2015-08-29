using HotChocolatey.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public class InstalledPackageLoader
    {
        private Task<List<ChocoItem>> loadingTask;

        public InstalledPackageLoader(ChocolateyController controller, ProgressIndication.IProgressIndicator progressIndicator)
        {
            loadingTask = controller.GetInstalled(progressIndicator);
        }

        public async Task<List<ChocoItem>> GetPackages()
        {
            return await loadingTask;
        }
    }
}
