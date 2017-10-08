using Microsoft.Win32;

namespace HotChocolatey.Model
{
    public static class WindowsStartup
    {
        private const string RunKeyName = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void ChangeStartup(string name, string command, bool enable)
        {
            if (enable)
            {
                if (!Exists(name))
                {
                    Enable(name, command);
                }
            }
            else
            {
                Disable(name);
            }
        }

        private static bool Exists(string appName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyName))
            {
                return key?.GetValue(appName) != null;
            }
        }

        private static void Enable(string appName, string executable)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyName, true))
            {
                key?.SetValue(appName, executable);
            }
        }

        private static void Disable(string appName)
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RunKeyName, true))
            {
                key?.DeleteValue(appName, false);
            }
        }
    }
}