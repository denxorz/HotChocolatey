using System;

namespace ChocolateyMilk
{
    public interface IFilter
    {
        Predicate<object> Filter { get; }
    }
}
