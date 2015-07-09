using System;

namespace HotChocolatey
{
    public interface IFilter
    {
        Predicate<object> Filter { get; }
    }
}
