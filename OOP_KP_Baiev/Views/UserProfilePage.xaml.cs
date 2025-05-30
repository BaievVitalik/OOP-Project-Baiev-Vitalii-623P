using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using OOP_KP_Baiev.Models;

namespace OOP_KP_Baiev.Views
{
    public partial class UserProfilePage : Page
    {
        private readonly Frame _frame;
        private readonly User _user;

        public UserProfilePage(User user, Frame frame)
        {
            InitializeComponent();
            _user = user;
            _frame = frame;
            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            ProfileTitleText.Text = _user switch
            {
                Freelancer => "Профіль фрілансера",
                Customer => "Профіль замовника",
                _ => "Профіль користувача"
        };

            FullNameText.Text = $"{_user.FirstName} {_user.LastName}".Trim();
            LoginText.Text = _user.Login;
            RoleText.Text = _user.UserType();
            BirthDateText.Text = _user.BirthDate?.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("uk-UA")) ?? "Не вказано";

            string location = $"{_user.Country}, {_user.City}".Trim(new char[] { ',', ' ' });
            LocationText.Text = string.IsNullOrWhiteSpace(location) ? "Не вказано" : location;
            DescriptionText.Text = string.IsNullOrWhiteSpace(_user.Description)
                ? "Опис відсутній"
                : _user.Description;

            AvatarImage.Source = LoadAvatar(_user.AvatarPath);
        }

        private BitmapImage LoadAvatar(string path)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = new Uri(path, UriKind.Absolute);
                    image.EndInit();
                    return image;
                }
            }
            catch { }

            return GetDefaultAvatar();
        }

        private BitmapImage GetDefaultAvatar()
        {
            try
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/default_avatar.png"));
            }
            catch
            {
                return null;
            }
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e) => _frame.GoBack();
    }
}
