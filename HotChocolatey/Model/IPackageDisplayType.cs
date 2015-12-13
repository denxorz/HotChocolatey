using HotChocolatey.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.ViewModel
{
    public interface IPackageDisplayType
    {
        bool HasMore { get; }

        Task<IEnumerable<Package>> GetMore(int numberOfItems);

        Task Refresh();

        Task ApplySearch(string search);
    }
}
