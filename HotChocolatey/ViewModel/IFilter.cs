using System;

namespace HotChocolatey.ViewModel
{
    public interface IFilter
    {
        Predicate<object> Filter { get; }
    }
}
