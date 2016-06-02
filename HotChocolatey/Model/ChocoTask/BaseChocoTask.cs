using System;
using System.Threading.Tasks;

namespace HotChocolatey.Model.ChocoTask
{
    internal abstract class BaseChocoTask
    {
        public async Task Execute(ChocoExecutor executor)
        {
            bool result = await executor.Execute($"{GetCommand()} --limitoutput {GetParameters()}", GetOutputLineCallback());
            AfterExecute(result);
        }

        protected abstract string GetCommand();
        protected abstract string GetParameters();
        protected abstract Action<string> GetOutputLineCallback();

        protected virtual void AfterExecute(bool result)
        {
            // 
        }
    }
}