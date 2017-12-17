using System;
using chocolatey.infrastructure.app.domain;

namespace HotChocolatey.Model.ChocoTask
{
    public class LoadSettingChocoTask : BaseChocoTask
    {
        public string Setting { get; private set; }

        public LoadSettingChocoTask(string name)
        {
            Config.CommandName = "config";
            Config.ConfigCommand.Name = name;
            Config.ConfigCommand.Command = ConfigCommandType.get;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback => Setting = outputLineCallback;
    }
}