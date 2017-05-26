using System;
using chocolatey.infrastructure.app.domain;

namespace HotChocolatey.Model.ChocoTask
{
    internal class SaveFeatureChocoTask : BaseChocoTask
    {
        public SaveFeatureChocoTask(string name, bool enable)
        {
            Config.CommandName = "feature";
            Config.FeatureCommand.Command = enable ? FeatureCommandType.enable : FeatureCommandType.disable;
            Config.FeatureCommand.Name = name;
        }

        protected override Action<string> GetOutputLineCallback() => outputLineCallback => { };
    }
}