using NuGet;
using System;

namespace HotChocolatey.Model
{
    public interface IAction
    {
        void Execute(ChocoExecutor chocoExecutor, SemanticVersion specificVersion, Action<string> outputLineCallback);
    }
}
