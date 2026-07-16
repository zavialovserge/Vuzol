using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Vuzol.Converteres
{
    public class TextWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string text = value as string ?? string.Empty;

            var formattedText = new FormattedText(
                text,
                culture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                14, // розмір шрифту — має збігатись з реальним
                Brushes.Black,
                VisualTreeHelper.GetDpi(Application.Current.MainWindow).PixelsPerDip);

            return formattedText.WidthIncludingTrailingWhitespace + 20; // + запас (padding)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

}
