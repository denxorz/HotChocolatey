using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using HotChocolatey.Model;
using NuGet;

namespace HotChocolatey.Administrative
{
    public class AdministrativeCommander : IAdministrativeCommandAcceptorCallback
    {
        private readonly Action<string> callback;

        public AdministrativeCommander(Action<string> callback)
        {
            this.callback = callback;
        }

        public void OutputCallback(string line)
        {
            callback.Invoke(line);
        }

        public async Task Install(bool includePreReleases, Package[] packages, SemanticVersion specificVersion)
        {
            (var factory, var proxy) = CreateClient();
            await Task<bool>.Factory.FromAsync(proxy.BeginInstall, proxy.EndInstall, includePreReleases, packages.Select(p => p.Id).ToArray(), specificVersion, null).ConfigureAwait(false);
            factory.Close();
        }

        public async Task Uninstall(Package[] packages)
        {
            (var factory, var proxy) = CreateClient();
            await Task<bool>.Factory.FromAsync(proxy.BeginUninstall, proxy.EndUninstall, packages.Select(p => p.Id).ToArray(), null).ConfigureAwait(false);
            factory.Close();
        }

        public async Task Update(bool includePreReleases, Package[] packages, SemanticVersion specificVersion)
        {
            (var factory, var proxy) = CreateClient();
            await Task<bool>.Factory.FromAsync(proxy.BeginUpdate, proxy.EndUpdate, includePreReleases, packages.Select(p => p.Id).ToArray(), specificVersion, null).ConfigureAwait(false);
            factory.Close();
        }

        public void Die()
        {
            (var factory, var proxy) = CreateClient();
            proxy.Die();
            factory.Close();
        }

        private (DuplexChannelFactory<IAdministrativeCommandAcceptor>, IAdministrativeCommandAcceptor) CreateClient()
        {
            var factory = new DuplexChannelFactory<IAdministrativeCommandAcceptor>(new InstanceContext(this),
                new NetNamedPipeBinding(),
                new EndpointAddress(AdministrativeCommandAcceptor.PipeAddress + AdministrativeCommandAcceptor.PipeName));
            var proxy = factory.CreateChannel();
            return (factory, proxy);
        }
    }
}