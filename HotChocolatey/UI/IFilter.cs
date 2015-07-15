using System;

namespace HotChocolatey.UI
{
    public interface IFilter
    {
        Predicate<object> Filter { get; }
    }
}
