using NuGet;
using System;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public interface IAction
    {
        Task ExecuteAsync(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback);
    }
}
