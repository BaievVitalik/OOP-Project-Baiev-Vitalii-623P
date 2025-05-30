using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using OOP_KP_Baiev.Models;

namespace OOP_KP_Baiev.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string statusString = value?.ToString();

            return statusString switch
            {
                "Active" => new SolidColorBrush(Color.FromRgb(34, 197, 94)),    
                "Completed" => new SolidColorBrush(Color.FromRgb(239, 68, 68)), 
                "Pending" => new SolidColorBrush(Color.FromRgb(234, 179, 8)),  
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
