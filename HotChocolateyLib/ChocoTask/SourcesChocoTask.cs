using chocolatey.infrastructure.app.domain;
using System;
using System.Collections.Generic;

namespace HotChocolatey.Model.ChocoTask
{
    public class SourcesChocoTask : BaseChocoTask
    {
        public List<string> Sources { get; } = new List<string>();

        public SourcesChocoTask()
        {
            Config.CommandName = "sources";
            Config.SourceCommand.Command = SourceCommandType.list;
        }

        protected override Action<string> GetOutputLineCallback() => parseSource;

        private void parseSource(string chocoOutput)
        {
            var tmp = chocoOutput.Split('|', ' ');
            if (tmp.Length < 2) return;
            Sources.Add(tmp[1].Trim());
        }
    }
}