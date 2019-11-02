using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using HotChocolatey.Model.ChocoTask;
using NuGet;

namespace HotChocolatey.Administrative
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    internal class AdministrativeCommandAcceptorService : IAdministrativeCommandAcceptor
    {
        public IAsyncResult BeginInstall(bool includePreReleases, string[] packageIds, SemanticVersion specificVersion, AsyncCallback callback, object state)
        {
            var client = GetClient();
            var task = Task<bool>.Factory.StartNew(o =>
            {
                void OutputLineCallback(string s)
                {
                    client.OutputCallback(s);
                }

                packageIds
                .Select(p => new InstallChocoTask(OutputLineCallback, includePreReleases, p, specificVersion))
                .ToList()
                .ForEach(t => t.Execute());

                return true;
            }, state);
            return task.ContinueWith(res => callback(task));
        }

        public bool EndInstall(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public IAsyncResult BeginUninstall(string[] packageIds, AsyncCallback callback, object state)
        {
            var client = GetClient();
            var task = Task<bool>.Factory.StartNew(o =>
            {
                void OutputLineCallback(string s)
                {
                    client.OutputCallback(s);
                }

                packageIds
                .Select(p => new UninstallChocoTask(OutputLineCallback, p))
                .ToList()
                .ForEach(t => t.Execute());

                return true;
            }, state);
            return task.ContinueWith(res => callback(task));
        }

        public bool EndUninstall(IAsyncResult result)
        {
            return ((Task<bool>)result).Result;
        }

        public IAsyncResult BeginUpdate(bool includePreReleases, string[] packageIds, SemanticVersion specificVersion, AsyncCallback callback, object state)
        {
            var client = GetClient();
            var task = Task<bool>.Factory.StartNew(o =>
            {
                void OutputLineCallback(string s)
                {
                    client.OutputCallback(s);
                }

                packageIds
                 .Select(p => new UpgradeChocoTask(OutputLineCallback, includePreReleases, p, specificVersion))
                 .ToList()
                 .ForEach(t => t.Execute());

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