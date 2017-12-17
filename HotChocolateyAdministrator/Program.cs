using System;
using System.Reflection;
using HotChocolatey.Administrative;
using HotChocolatey.Utility;

namespace HotChocolateyAdministrator
{
    class Program
    {
        static void Main()
        {
            Log.ResetSettings(true, true);
            Log.Info($@"---
Version:{Assembly.GetExecutingAssembly().GetName().Version}
MachineName:{Environment.MachineName}
OSVersion:{Environment.OSVersion}
Is64BitOperatingSystem:{Environment.Is64BitOperatingSystem}");

            AppDomain.CurrentDomain.UnhandledException += (s, e) => Log.Error($"UnhandledException: {e.ExceptionObject}");

            AdministrativeCommandAcceptor.StartListeningForCommands();
        }
    }
}
