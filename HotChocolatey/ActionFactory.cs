using System.Collections.Generic;
using System.Threading.Tasks;
using NuGet;
using System.Linq;

namespace HotChocolatey
{
    internal class ActionFactory
    {
        internal static List<IAction> Generate(ChocolateyController controller, ChocoItem chocoItem)
        {
            var actions = new List<IAction>();

            if (chocoItem.IsInstalled)
            {
                if (chocoItem.IsUpgradable)
                {
                    actions.Add(new UpgradeAction(controller, chocoItem));
                }

                actions.Add(new UninstallAction(controller, chocoItem));
            }
            else
            {
                actions.Add(new InstallAction(controller, chocoItem));
            }

            return actions;
        }

        private class InstallAction : IAction
        {
            private ChocolateyController controller;
            private ChocoItem chocoItem;

            public InstallAction(ChocolateyController controller, ChocoItem chocoItem)
            {
                this.controller = controller;
                this.chocoItem = chocoItem;
            }

            public string Name { get; } = "Install";
            public List<SemanticVersion> Versions => chocoItem.Versions;

            async Task IAction.Execute(SemanticVersion specificVersion)
            {
                await controller.Install(chocoItem, specificVersion);
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

            public UninstallAction(ChocolateyController controller, ChocoItem chocoItem)
            {
                this.controller = controller;
                this.chocoItem = chocoItem;
            }

            public string Name { get; } = "Uninstall";
            public List<SemanticVersion> Versions => new List<SemanticVersion> { chocoItem.InstalledVersion };

            async Task IAction.Execute(SemanticVersion specificVersion)
            {
                await controller.Uninstall(chocoItem);
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

            public UpgradeAction(ChocolateyController controller, ChocoItem chocoItem)
            {
                this.controller = controller;
                this.chocoItem = chocoItem;
            }

            public string Name { get; } = "Upgrade";
            public List<SemanticVersion> Versions =>  chocoItem.Versions.Where(t => t > chocoItem.InstalledVersion).ToList();
 
            async Task IAction.Execute(SemanticVersion specificVersion)
            {
                await controller.Upgrade(chocoItem);
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}