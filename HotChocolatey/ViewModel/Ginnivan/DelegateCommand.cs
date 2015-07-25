using System;

namespace HotChocolatey.ViewModel.Ginnivan
{
    /// <summary>
    /// https://gist.github.com/JakeGinnivan/5166898
    /// </summary>
    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action executeMethod)
            : base(o => executeMethod())
        { }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        { }
    }
}
