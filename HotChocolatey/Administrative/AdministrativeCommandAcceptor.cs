using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;

namespace HotChocolatey.Administrative
{
    public static class AdministrativeCommandAcceptor
    {
        public const string PipeAddress = "net.pipe://localhost/";
        public const string PipeName = "HotChocolatey\\AdministrativeCommandAcceptor";
        public static ManualResetEvent M { get; private set; }

        public static void StartListeningForCommands()
        {
            using (M = new ManualResetEvent(false))
            using (var host = new ServiceHost(typeof(AdministrativeCommandAcceptorService), new Uri(PipeAddress)))
            {
                host.AddServiceEndpoint(typeof(IAdministrativeCommandAcceptor), new NetNamedPipeBinding(), PipeName);
                host.Description.Behaviors.Remove(typeof(ServiceDebugBehavior));
                host.Description.Behaviors.Add(new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
                host.Open();
                M.WaitOne();
                host.Close();
            }
        }
    }
}