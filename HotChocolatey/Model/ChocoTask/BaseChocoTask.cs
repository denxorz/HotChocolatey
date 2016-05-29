using System;
using System.Threading.Tasks;

namespace HotChocolatey.Model.ChocoTask
{
    internal abstract class BaseChocoTask
    {
        public async Task Execute(ChocoExecutor executor, Action<string> outputLineCallback)
        {
            bool result = await executor.Execute($"{GetCommand()} --limitoutput {GetParameters()}", outputLineCallback);
            AfterExecute(result);
        }

        protected abstract string GetCommand();
        protected abstract string GetParameters();

        protected virtual void AfterExecute(bool result)
        {
            // 
        }
    }
}