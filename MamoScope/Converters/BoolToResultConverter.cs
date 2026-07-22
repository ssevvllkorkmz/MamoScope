using System;
using System.Globalization;
using System.Windows.Data;

namespace MamoScope.Converters
{
    public class BoolToResultConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool basariliMi)
                return basariliMi ? "Başarılı" : "Başarısız";

            return string.Empty;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Bu alan geri dönüştürülemez, sadece görüntüleme amaçlıdır.");
        }
    }
}