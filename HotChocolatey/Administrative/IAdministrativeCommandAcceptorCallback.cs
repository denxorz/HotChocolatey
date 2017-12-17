using System.ServiceModel;

namespace HotChocolatey.Administrative
{
    public interface IAdministrativeCommandAcceptorCallback
    {
        [OperationContract(IsOneWay = true)]
        void OutputCallback(string line);
    }
}