using NuGet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotChocolatey.Model
{
    public interface IAction
    {
        Task Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback);

        string Name { get; }
        List<SemanticVersion> Versions { get; }
    }
}
