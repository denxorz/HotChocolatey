using HotChocolatey.Model;
using System;
using System.Collections.Generic;

namespace HotChocolatey.ViewModel
{
    public static class FilterFactory
    {
        public static List<IFilter> BuildFilters(ChocolateyController controller, ProgressIndication.IProgressIndicator progressIndicator) => new List<IFilter>
            {
                new NoFilter(controller, progressIndicator),
                new InstalledFilter(controller),
                new InstalledUpgradableFilter(controller),
            };

        public static IFilter BuildUpgradeFilter(ChocolateyController controller) => new InstalledUpgradableFilter(controller);

        private class NoFilter : IFilter
        {
            private ChocolateyController controller;
            private ProgressIndication.IProgressIndicator progressIndicator;

            public NoFilter(ChocolateyController controller, ProgressIndication.IProgressIndicator progressIndicator)
            {
                this.controller = controller;
                this.progressIndicator = progressIndicator;
            }

            public override string ToString() => "All";
            public IPackageList CreatePackageList() => new AllPackageList(controller, progressIndicator);
        }

        private class InstalledFilter : IFilter
        {
            private ChocolateyController controller;

            public InstalledFilter(ChocolateyController controller)
            {
                this.controller = controller;
            }

            public override string ToString() => "Installed";
            public IPackageList CreatePackageList() => new InstalledPackageList(controller);
        }

        private class InstalledUpgradableFilter : IFilter
        {
            private ChocolateyController controller;

            public InstalledUpgradableFilter(ChocolateyController controller)
            {
                this.controller = controller;
            }

            public override string ToString() => "Upgradable available";
            public IPackageList CreatePackageList() => new UpgradablePackageList(controller);
        }
    }
}
