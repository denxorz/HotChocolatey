using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace HotChocolatey.Administrative
{
    public class AdministrativeCommanderProvider
    {
        private Process administrativeProcess;

        public AdministrativeCommander Create(Action<string> outputLineCallback)
        {
            StartAdmin();
            return new AdministrativeCommander(outputLineCallback);
        }

        public void Close()
        {
            if (!(administrativeProcess?.HasExited ?? true))
            {
                new AdministrativeCommander(a => { }).Die();
            }
        }

        private void StartAdmin()
        {
            if (administrativeProcess?.HasExited ?? true)
            {
                administrativeProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = GetFileName(),
                        Verb = "runas",
                        CreateNoWindow = true,
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                administrativeProcess.Start();

                Thread.Sleep(100);
            }
        }

        private static string GetFileName()
        {
            return Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "HotChocolateyAdministrator.exe");
        }
    }
}