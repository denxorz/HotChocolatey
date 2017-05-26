using System;
using System.Collections.Generic;
using chocolatey.infrastructure.app.domain;

namespace HotChocolatey.Model.ChocoTask
{
    internal class LoadFeaturesChocoTask : BaseChocoTask
    {
        public Dictionary<string, bool> Features { get; } = new Dictionary<string, bool>();

        public LoadFeaturesChocoTask()
        {
            Config.CommandName = "feature";
            Config.FeatureCommand.Command = FeatureCommandType.list;
        }

        protected override Action<string> GetOutputLineCallback() => ParseFeature;

        private void ParseFeature(string outputLine)
        {
            var feature = outputLine.Split('|', '-');
            Features[feature[0].Trim()] = feature[1].Trim() == "[Enabled]";
        }
    }
}