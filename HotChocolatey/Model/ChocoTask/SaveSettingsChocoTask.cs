using System;

namespace HotChocolatey.Model.ChocoTask
{
    internal class SaveSettingsChocoTask : BaseChocoTask
    {
        private readonly string name;
        private readonly object value;

        public SaveSettingsChocoTask(string name, object value)
        {
            this.name = name;
            this.value = value;
        }

        protected override string GetCommand() => "config set";
        protected override Action<string> GetOutputLineCallback() => outputLineCallback => { };
        protected override string GetParameters() => $"--name {name} --value {value}";
    }
}