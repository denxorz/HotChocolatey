using System;
using System.Collections.Generic;

namespace HotChocolatey.Model.ChocoTask
{
    internal class LoadFeaturesChocoTask : BaseChocoTask
    {
        public Dictionary<string, bool> Features { get; } = new Dictionary<string, bool>();

        protected override string GetCommand() => "features";
        protected override Action<string> GetOutputLineCallback() => ParseFeature;
        protected override string GetParameters() => string.Empty;

        private void ParseFeature(string outputLine)
        {
            var feature = outputLine.Split('|', '-');
            Features[feature[0].Trim()] = feature[1].Trim() == "[Enabled]";
        }
    }
}