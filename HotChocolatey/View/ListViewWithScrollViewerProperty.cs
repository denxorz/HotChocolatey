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
                    Decorator border = (Decorator)VisualTreeHelper.GetChild(this, 0);
                    return border.Child as ScrollViewer;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
