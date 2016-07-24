using HotChocolatey.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.ViewModel
{
    public interface IPackageDisplayType
    {
        bool HasMore { get; }

        Task<IEnumerable<Package>> GetMoreAsync(int numberOfItems);

        Task RefreshAsync();

        Task ApplySearchAsync(string search);
    }
}
