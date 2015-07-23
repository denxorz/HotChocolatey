using NuGet;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public interface IAction
    {
        Task Execute(SemanticVersion specificVersion);
        string Name { get; }
        List<SemanticVersion> Versions { get; }
    }
}