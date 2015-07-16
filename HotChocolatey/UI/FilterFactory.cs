using HotChocolatey.Logic;
using System;
using System.Collections.Generic;

namespace HotChocolatey.UI
{
    public static class FilterFactory
    {
        public static List<IFilter> AvailableFilters => new List<IFilter>
            {
                new NoFilter(),
                new InstalledFilter(),
                new InstalledUpgradableFilter(),
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
            public override string ToString() => "Upgradable available";
        }
    }
}
