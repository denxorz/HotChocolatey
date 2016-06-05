using System;
using System.IO;
using System.Windows.Controls;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace HotChocolatey.View
{
    /// <summary>
    /// http://ourcodeworld.com/articles/read/27/create-notification-in-windows-10-control-center-style-with-c-
    /// </summary>
    public partial class NotificationManager : UserControl
    {
        public const string APP_ID = "Denxorz.HotChocolatey";

        public NotificationManager()
        {
            InitializeComponent();
        }

        public ToastNotification CreateNotification(string text)
        {
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements[0].AppendChild(toastXml.CreateTextNode(text));

            string imagePath = "file:///" + Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images/Hot Chocolate-100.png");
            XmlNodeList imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            return new ToastNotification(toastXml);
        }

        public void Show(ToastNotification notification)
        {
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(notification);
        }
    }
}
