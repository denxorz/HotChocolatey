using System;
using System.ServiceModel;
using NuGet;

namespace HotChocolatey.Administrative
{
    [ServiceContract(CallbackContract = typeof(IAdministrativeCommandAcceptorCallback))]
    public interface IAdministrativeCommandAcceptor
    {
        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginInstall(bool includePreReleases, string[] packageIds, SemanticVersion specificVersion, AsyncCallback callback, object state);

        bool EndInstall(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUninstall(string[] packageIds, AsyncCallback callback, object state);

        bool EndUninstall(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        IAsyncResult BeginUpdate(bool includePreReleases, string[] packageIds, SemanticVersion specificVersion, AsyncCallback callback, object state);

        bool EndUpdate(IAsyncResult result);

        [OperationContract]
        void Die();
    }
}