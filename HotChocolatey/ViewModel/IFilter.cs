using HotChocolatey.Model;

namespace HotChocolatey.ViewModel
{
    public interface IFilter
    {
        IPackageList CreatePackageList();
    }
}
