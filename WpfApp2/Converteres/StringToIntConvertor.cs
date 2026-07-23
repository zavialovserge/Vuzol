using System.Windows.Data;

namespace Vuzol.Converteres
{
    [ValueConversion(typeof(int), typeof(string))]
    public class StringToIntConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int returnedValue;

            if (int.TryParse((string)value, out returnedValue))
            {
                return returnedValue;
            }
            return "";
        }
    }
}
