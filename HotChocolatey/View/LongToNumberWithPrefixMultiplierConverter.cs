using System;
using System.Globalization;
using System.Windows.Data;

namespace HotChocolatey.View
{
    public class LongToNumberWithPrefixMultiplierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var number = (int)value;
            return HumanReadableNumber(number);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// http://stackoverflow.com/a/3758880
        /// </summary>
        private static string HumanReadableNumber(long number)
        {
            if (number < 1000)
            {
                return number.ToString();
            }

            int exp = (int)(Math.Log(number) / Math.Log(1000));
            char pre = "KMGTPE"[(exp - 1)];
            return $"{(number / Math.Pow(1000, exp)):#}{pre}";
        }
    }
}
