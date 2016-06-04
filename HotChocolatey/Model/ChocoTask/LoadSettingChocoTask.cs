using System;

namespace HotChocolatey.Model.ChocoTask
{
    internal class LoadSettingChocoTask : BaseChocoTask
    {
        public string Setting { get; private set; }

        private readonly string name;

        public LoadSettingChocoTask(string name)
        {
            this.name = name;
        }

        protected override string GetCommand() => "config get";
        protected override Action<string> GetOutputLineCallback() => outputLineCallback => Setting = outputLineCallback;
        protected override string GetParameters() => $"--name {name}";
    }
}