using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public interface IPackageList
    {
        bool HasMore { get; }
        Task Refresh();
        Task<IEnumerable<ChocoItem>> GetMore(int numberOfItems);
        Task ApplySearch(string searchFor);
    }
}
