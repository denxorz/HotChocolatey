using System;
using chocolatey.infrastructure.app.configuration;
using chocolatey.infrastructure.app.runners;
using chocolatey.infrastructure.registration;

namespace HotChocolatey.Model.ChocoTask
{
    public abstract class BaseChocoTask
    {
        protected readonly ChocolateyConfiguration Config = new ChocolateyConfiguration();

        public void Execute()
        {
            bool result = Execute(GetOutputLineCallback());
            AfterExecute(result);
        }

        protected abstract Action<string> GetOutputLineCallback();

        private bool Execute(Action<string> outputLineCallback)
        {
            var console = new GenericRunner();
            var container = SimpleInjectorContainer.Container;

            Config.RegularOutput = false;
            Config.Sources = "https://chocolatey.org/api/v2/";

            var chocoCommunication = new ChocoCommunication(outputLineCallback);
            chocolatey.infrastructure.logging.Log.InitializeWith(chocoCommunication);
            console.run(Config, container, false, command => { System.Diagnostics.Debug.WriteLine($"Command {Config.CommandName} requires admin: {command.may_require_admin_access()}"); });

            return chocoCommunication.IsSuccess;
        }


        protected virtual void AfterExecute(bool result)
        {
            //
        }
    }
}