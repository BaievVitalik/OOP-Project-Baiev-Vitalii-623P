using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OOP_KP_Baiev.Views
{
    public partial class AccountPage : Page
    {
        private readonly Frame _frame;
        private readonly User _user;

        public AccountPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;

            var currentUser = App.Current.Properties["CurrentUser"] as User;
            if (currentUser == null)
            {
                MessageBox.Show("Помилка завантаження профілю. Поточний користувач не визначений.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                _frame.Navigate(new LoginPage(_frame));
                return;
            }
            _user = currentUser;

            LoadUserInfo();
        }

        private void LoadUserInfo()
        {
            string fullName = $"{_user.FirstName} {_user.LastName}".Trim();
            FullNameText.Text = string.IsNullOrWhiteSpace(fullName) ? _user.Login : fullName;

            UsernameText.Text = _user.Login;
            RoleText.Text = _user.UserType();

            BirthDateText.Text = _user.BirthDate.HasValue
                ? _user.BirthDate.Value.ToString("dd MMMM yyyy", new System.Globalization.CultureInfo("uk-UA")) // добавил yyyy для полноты
                : "Не вказано";

            string country = string.IsNullOrWhiteSpace(_user.Country) ? "" : _user.Country;
            string city = string.IsNullOrWhiteSpace(_user.City) ? "" : _user.City;
            string location = $"{country}, {city}".Trim(new char[] { ',', ' ' });
            LocationText.Text = string.IsNullOrWhiteSpace(location) ? "Не вказано" : location;

            DescriptionText.Text = string.IsNullOrWhiteSpace(_user.Description)
                ? "Опис відсутній" 
                : _user.Description;

            BitmapImage avatarImageSource = null; 

            if (!string.IsNullOrEmpty(_user.AvatarPath) && File.Exists(_user.AvatarPath))
            {
                try
                {
                    avatarImageSource = new BitmapImage();
                    avatarImageSource.BeginInit();
                    avatarImageSource.CacheOption = BitmapCacheOption.OnLoad;
                    avatarImageSource.UriSource = new Uri(_user.AvatarPath, UriKind.Absolute);
                    avatarImageSource.EndInit();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Помилка завантаження аватара: {ex.Message}");
                    avatarImageSource = GetDefaultAvatar();
                }
            }
            else
            {
                avatarImageSource = GetDefaultAvatar();
            }

            if (this.FindName("AvatarImage") is Image avatarImageControl)
            {
                avatarImageControl.Source = avatarImageSource;
            }

        }

        private BitmapImage GetDefaultAvatar()
        {
            try
            {
                return new BitmapImage(new Uri("pack://application:,,,/Resources/default_avatar.png"));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка завантаження стандартного аватара: {ex.Message}");
                return null; 
            }
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new ProfileEditPage(_frame, _user));
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new MainPage(_frame)); 
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["CurrentUser"] = null;
            _frame.Navigate(new LoginPage(_frame));
        }
    }
}