using HotChocolatey.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.ViewModel
{
    public interface IPackageDisplayType
    {
        bool HasMore { get; }

        IEnumerable<Package> GetMore(int numberOfItems);

        Task RefreshAsync();

        Task ApplySearchAsync(string search);


        bool AllowsMultiSelect { get; }
        bool AllowsMultiSelectInstall { get; }
        bool AllowsMultiSelectUpdate { get; }
        bool AllowsMultiSelectUninstall { get; }
    }
}
