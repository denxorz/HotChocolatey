using System;
using System.Collections.Generic;

namespace HotChocolatey
{
    public static class FilterFactory
    {
        public static List<IFilter> AvailableFilters => new List<IFilter>
            {
                new NoFilter(),
                new InstalledFilter(),
                new InstalledUpgradableFilter(),
                new NotInstalledFilter(),
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
            public Predicate<object> Filter => t => (t as ChocoItem).IsUpgradable;
            public override string ToString() => "Upgradable";
        }

        private class NotInstalledFilter : IFilter
        {
            public Predicate<object> Filter => t => !(t as ChocoItem).IsInstalled;
            public override string ToString() => "Not installed";
        }
    }
}
