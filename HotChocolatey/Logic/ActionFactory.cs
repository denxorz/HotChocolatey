using NuGet;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotChocolatey.Logic
{
    internal class ActionFactory
    {
        internal static List<IAction> Generate(ChocolateyController controller, ChocoItem chocoItem, UI.ProgressIndication.IProgressIndicator progressIndicator)
        {
            var actions = new List<IAction>();

            if (chocoItem.IsInstalled)
            {
                if (chocoItem.IsUpgradable)
                {
                    actions.Add(new UpgradeAction(controller, chocoItem, progressIndicator));
                }

                actions.Add(new UninstallAction(controller, chocoItem, progressIndicator));
            }
            else
            {
                actions.Add(new InstallAction(controller, chocoItem, progressIndicator));
            }

            return actions;
        }

        private class InstallAction : IAction
        {
            private ChocolateyController controller;
            private ChocoItem chocoItem;
            private UI.ProgressIndication.IProgressIndicator progressIndicator;

            public InstallAction(ChocolateyController controller, ChocoItem chocoItem, UI.ProgressIndication.IProgressIndicator progressIndicator)
            {
                this.controller = controller;
                this.chocoItem = chocoItem;
                this.progressIndicator = progressIndicator;
            }

            public string Name { get; } = "Install";
            public List<SemanticVersion> Versions => chocoItem.Versions;

            async Task IAction.Execute(SemanticVersion specificVersion)
            {
                using (new UI.ProgressIndication(progressIndicator))
                {
                    await controller.Install(chocoItem, specificVersion);
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class UninstallAction : IAction
        {
            private ChocolateyController controller;
            private ChocoItem chocoItem;
            private UI.ProgressIndication.IProgressIndicator progressIndicator;

            public UninstallAction(ChocolateyController controller, ChocoItem chocoItem, UI.ProgressIndication.IProgressIndicator progressIndicator)
            {
                this.controller = controller;
                this.chocoItem = chocoItem;
                this.progressIndicator = progressIndicator;
            }

            public string Name { get; } = "Uninstall";
            public List<SemanticVersion> Versions => new List<SemanticVersion> { chocoItem.InstalledVersion };

            async Task IAction.Execute(SemanticVersion specificVersion)
            {
                using (new UI.ProgressIndication(progressIndicator))
                {
                    await controller.Uninstall(chocoItem);
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class UpgradeAction : IAction
        {
            private ChocolateyController controller;
            private ChocoItem chocoItem;
            private UI.ProgressIndication.IProgressIndicator progressIndicator;

            public UpgradeAction(ChocolateyController controller, ChocoItem chocoItem, UI.ProgressIndication.IProgressIndicator progressIndicator)
            {
                this.controller = controller;
                this.chocoItem = chocoItem;
                this.progressIndicator = progressIndicator;
            }

            public string Name { get; } = "Upgrade";
            public List<SemanticVersion> Versions =>  chocoItem.Versions.Where(t => t > chocoItem.InstalledVersion).ToList();
 
            async Task IAction.Execute(SemanticVersion specificVersion)
            {
                using (new UI.ProgressIndication(progressIndicator))
                {
                    await controller.Upgrade(chocoItem);
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}