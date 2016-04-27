using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotChocolatey.View
{
    public class ListViewWithScrollViewerProperty : ListView
    {
        public ScrollViewer ScrollViewer
        {
            get
            {
                try
                {
                    Decorator border = VisualTreeHelper.GetChild(this, 0) as Decorator;
                    return border.Child as ScrollViewer;
                }
                catch
                {
                    return null;
                }
            }
        }

        private ScrollViewer FindScrollViewer(DependencyObject d)
        {
            if (d is ScrollViewer)
            {
                return d as ScrollViewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(d); i++)
            {
                var sw = FindScrollViewer(VisualTreeHelper.GetChild(d, i));
                if (sw != null)
                {
                    return sw;
                }
            }
            return null;
        }
    }
}
