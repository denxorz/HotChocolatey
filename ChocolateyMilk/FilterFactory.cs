using System;
using System.Collections.Generic;

namespace ChocolateyMilk
{
    public static class FilterFactory
    {
        public static List<IFilter> AvailableFilters => new List<IFilter>
            {
                new NoFilter(),
                new InstalledFilter(),
                new InstalledUpgradableFilter(),
                new NotInstalledFilter(),
                new MarkedForInstallationFilter(),
                new MarkedForUpgradeFilter(),
                new MarkedForUninstallFilter(),
            };


        private class NoFilter : IFilter
        {
            public Predicate<object> Filter => t => true;
            public override string ToString() => "All";
        }

        private class InstalledFilter : IFilter
        {
            public Predicate<object> Filter => t => (t as ChocoItem).IsInstalled;
            public override string ToString() => "Installed";
        }

        private class InstalledUpgradableFilter : IFilter
        {
            public Predicate<object> Filter => t => (t as ChocoItem).IsInstalledUpgradable;
            public override string ToString() => "Installed (upgradable)";
        }

        private class NotInstalledFilter : IFilter
        {
            public Predicate<object> Filter => t => !(t as ChocoItem).IsInstalled;
            public override string ToString() => "Not installed";
        }

        private class MarkedForInstallationFilter : IFilter
        {
            public Predicate<object> Filter => t => (t as ChocoItem).IsMarkedForInstallation;
            public override string ToString() => "Marked for installation";
        }

        private class MarkedForUpgradeFilter : IFilter
        {
            public Predicate<object> Filter => t => (t as ChocoItem).IsMarkedForUpgrade;
            public override string ToString() => "Marked for upgrade";
        }

        private class MarkedForUninstallFilter : IFilter
        {
            public Predicate<object> Filter => t => (t as ChocoItem).IsMarkedForUninstall;
            public override string ToString() => "Marked for removal";
        }
    }
}
