using System.Collections.ObjectModel;
using HotChocolatey.Model;

namespace HotChocolatey.ViewModel
{
    internal class ChocoCommunicationViewModel
    {
        public ObservableCollection<string> Communication => ChocoCommunication.Communication;
    }
}
