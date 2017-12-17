using System;
using System.Collections.Generic;
using SingleInstanceApp;

namespace HotChocolatey
{
    public partial class App : ISingleInstance
    {
        public static event EventHandler SecondInstanceStarted;

        private const string UniqueApplicationName = "HotChocolatey.Instance.3C4534D0-F14B-42D8-A552-6D611A3FEB5F";

        [STAThread]
        public static void Main()
        {
            if (!SingleInstance<App>.InitializeAsFirstInstance(UniqueApplicationName)) return;

            var application = new App();
            application.InitializeComponent();
            application.Run();

            SingleInstance<App>.Cleanup();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            SecondInstanceStarted?.Invoke(null, EventArgs.Empty);
            return true;
        }
    }
}
