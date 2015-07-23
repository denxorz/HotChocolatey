using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.Collections.Generic;
using System.IO;

namespace HotChocolatey.Utility
{
    static class Log
    {
        private static readonly ILog log = LogManager.GetLogger("default");

        /// <summary>
        /// http://stackoverflow.com/questions/16336917/can-you-configure-log4net-in-code-instead-of-using-a-config-file
        /// http://stackoverflow.com/questions/885378/log4net-pure-code-configuration-with-filter-in-c-sharp
        /// </summary>
        public static void ResetSettings(bool toLog, bool toConsole, bool toDiagnostics, ViewModel.Diagnostics diagnostics)
        {
            List<IAppender> appenders = new List<IAppender>();

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            if (toLog)
            {
                var roller = new RollingFileAppender();
                roller.AppendToFile = true;
                roller.File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "HotChocolatey", "log.txt");
                roller.Layout = patternLayout;
                roller.MaxSizeRollBackups = 5;
                roller.MaximumFileSize = "10MB";
                roller.RollingStyle = RollingFileAppender.RollingMode.Size;
                roller.StaticLogFileName = true;
                roller.ActivateOptions();
                appenders.Add(roller);
            }

            if (toConsole)
            {
                var console = new ConsoleAppender();
                console.Layout = patternLayout;
                appenders.Add(console);
            }

            if (toDiagnostics)
            {
                appenders.Add(diagnostics);
            }

            BasicConfigurator.Configure(appenders.ToArray());
        }

        public static void Error(string message, params object[] list)
        {
            log.ErrorFormat(message, list);
        }

        public static void Info(string message, params object[] list)
        {
            log.InfoFormat(message, list);
        }
    }
}
