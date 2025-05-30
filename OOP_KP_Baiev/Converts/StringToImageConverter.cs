using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OOP_KP_Baiev.Converters
{
    public class StringToImageConverter : IValueConverter
    {
        private static readonly string DefaultAvatarPath = "pack://application:,,,/Resources/Avatars/default_avatar.png";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;

            if (string.IsNullOrWhiteSpace(path))
                return new BitmapImage(new Uri(DefaultAvatarPath, UriKind.Absolute));

            try
            {
                return new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                return new BitmapImage(new Uri(DefaultAvatarPath, UriKind.Absolute));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
