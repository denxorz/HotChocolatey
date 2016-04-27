using System;
using System.Windows.Data;
using System.Windows.Shell;

namespace HotChocolatey.View
{
    internal class BooleanToTaskbarStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? TaskbarItemProgressState.Indeterminate : TaskbarItemProgressState.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
