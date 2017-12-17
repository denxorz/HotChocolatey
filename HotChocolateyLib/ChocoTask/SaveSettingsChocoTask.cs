using System;
using chocolatey.infrastructure.app.domain;

namespace HotChocolatey.Model.ChocoTask
{
    public class SaveSettingsChocoTask : BaseChocoTask
    {
        public SaveSettingsChocoTask(string name, object value)
        {
            Config.CommandName = "config";
            Config.ConfigCommand.Name = name;
            Config.ConfigCommand.ConfigValue = value.ToString();
            Config.ConfigCommand.Command = ConfigCommandType.set;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback => { };
    }
}