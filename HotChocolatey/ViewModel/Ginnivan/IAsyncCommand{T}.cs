using System.Threading.Tasks;
using System.Windows.Input;

namespace HotChocolatey.ViewModel.Ginnivan
{
    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
    {
        Task ExecuteAsync(T obj);

        bool CanExecute(object obj);

        ICommand Command { get; }
    }
}
