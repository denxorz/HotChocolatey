using HotChocolatey.Model;

namespace HotChocolatey.ViewModel
{
    public interface IFilter
    {
        PackageListBase CreatePackageList();
    }
}
