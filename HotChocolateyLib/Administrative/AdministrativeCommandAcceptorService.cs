using System;
using System.ServiceModel;
using System.Threading.Tasks;
using HotChocolatey.Model.ChocoTask;
using NuGet;

namespace HotChocolatey.Administrative
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class AdministrativeCommandAcceptorService : IAdministrativeCommandAcceptor
    {
        public IAsyncResult BeginInstall(bool includePreReleases, string packageId, SemanticVersion specificVersion, AsyncCallback callback, object state)
        {
            var client = GetClient();
            var task = Task<bool>.Factory.StartNew(o =>
            {
                void OutputLineCallback(string s)
                {
                    client.OutputCallback(s);
                }

                new InstallChocoTask(OutputLineCallback, includePreReleases, packageId, specificVersion).Execute();
                return true;
            }, state);
            return task.ContinueWith(res => callback(task));
        }

        public bool EndInstall(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public IAsyncResult BeginUninstall(string packageId, AsyncCallback callback, object state)
        {
            var client = GetClient();
            var task = Task<bool>.Factory.StartNew(o =>
            {
                void OutputLineCallback(string s)
                {
                    client.OutputCallback(s);
                }

                new UninstallChocoTask(OutputLineCallback, packageId).Execute();
                return true;
            }, state);
            return task.ContinueWith(res => callback(task));
        }

        public bool EndUninstall(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public IAsyncResult BeginUpdate(bool includePreReleases, string packageId, SemanticVersion specificVersion, AsyncCallback callback, object state)
        {
            var client = GetClient();
            var task = Task<bool>.Factory.StartNew(o =>
            {
                void OutputLineCallback(string s)
                {
                    client.OutputCallback(s);
                }

                new UpgradeChocoTask(OutputLineCallback, includePreReleases, packageId, specificVersion).Execute();
                return true;
            }, state);
            return task.ContinueWith(res => callback(task));
        }

        public bool EndUpdate(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public void Die()
        {
            AdministrativeCommandAcceptor.M.Set();
        }

        private IAdministrativeCommandAcceptorCallback GetClient()
        {
            return OperationContext.Current.GetCallbackChannel<IAdministrativeCommandAcceptorCallback>();
        }
    }
}