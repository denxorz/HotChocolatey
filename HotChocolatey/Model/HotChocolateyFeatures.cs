using System.Collections.Generic;
using System.Reflection;

namespace HotChocolatey.Model
{
    class HotChocolateyFeatures
    {
        private const string ShowNotifications = "Show notifications";
        private const string ExitToTray = "Exit to tray";
        private const string StartWithWindows = "Start with windows";

        public IReadOnlyList<ChocoFeature> LoadFeatures()
        {
            return new[]
            {
                new ChocoFeature(ShowNotifications, "Show a notification in the Windows Notification Center when updates are found.", Properties.Settings.Default.ShowNotifications),
                new ChocoFeature(ExitToTray, "Keep the application running in the background/tray when the window is closed.", Properties.Settings.Default.ExitToTray),
                new ChocoFeature(StartWithWindows, "Startup the application when Windows starts.", Properties.Settings.Default.StartWithWindows),
            };
        }

        public void SaveFeature(ChocoFeature feature)
        {
            if (feature.Name == ShowNotifications)
            {
                Properties.Settings.Default.ShowNotifications = feature.IsEnabled;
                Properties.Settings.Default.Save();
            }
            else if (feature.Name == ExitToTray)
            {
                Properties.Settings.Default.ExitToTray = feature.IsEnabled;
                Properties.Settings.Default.Save();
            }
            else if (feature.Name == StartWithWindows)
            {
                Properties.Settings.Default.StartWithWindows = feature.IsEnabled;
                Properties.Settings.Default.Save();

                WindowsStartup.ChangeStartup("HotChocolatey", Assembly.GetExecutingAssembly().Location, feature.IsEnabled);
            }
        }
    }
}
