using System;

namespace ChocolateyMilk
{
    public interface IFilter
    {
        Predicate<object> Filter { get; }
    }

    public class NoFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => true;
        public override string ToString() => "All";
    }

    public class InstalledFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => (t as ChocoItem).IsInstalled;
        public override string ToString() => "Installed";
    }

    public class InstalledUpgradableFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => (t as ChocoItem).IsInstalledUpgradable;
        public override string ToString() => "Installed (upgradable)";
    }

    public class NotInstalledFilter : IFilter
    {
        public Predicate<object> Filter { get; } = t => !(t as ChocoItem).IsInstalled;
        public override string ToString() => "Not installed";
    }
}
