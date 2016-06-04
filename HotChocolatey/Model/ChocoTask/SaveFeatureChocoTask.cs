using System;

namespace HotChocolatey.Model.ChocoTask
{
    internal class SaveFeatureChocoTask : BaseChocoTask
    {
        private readonly string name;
        private readonly bool enable;

        public SaveFeatureChocoTask(string name, bool enable)
        {
            this.name = name;
            this.enable = enable;
        }

        protected override string GetCommand() => $"feature {(enable ? "enable" : "disable")}";
        protected override Action<string> GetOutputLineCallback() => outputLineCallback => { };
        protected override string GetParameters() => $"-n {name}";
    }
}