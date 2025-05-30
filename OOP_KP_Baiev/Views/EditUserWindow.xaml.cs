using Microsoft.Win32;
using System.Windows;
using System.Windows.Media.Imaging;

namespace OOP_KP_Baiev.Views
{
    public partial class EditUserWindow : Window
    {
        private User user;
        private string avatarPath;
        private readonly string defaultAvatarPath = "/Assets/default-avatar.png"; // относительный путь к дефолтному аватару

        public EditUserWindow(User userToEdit)
        {
            InitializeComponent();

            user = userToEdit;
            LoadUserData();
        }

        private void LoadUserData()
        {
            LoginBox.Text = user.Login;
            EmailBox.Text = user.Email;
            FirstNameBox.Text = user.FirstName;
            LastNameBox.Text = user.LastName;
            CountryBox.Text = user.Country;
            CityBox.Text = user.City;
            BirthDatePicker.SelectedDate = user.BirthDate;
            DescriptionBox.Text = user.Description;

            avatarPath = user.AvatarPath ?? defaultAvatarPath;
            SetAvatar(avatarPath);
        }

        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                avatarPath = openFileDialog.FileName;
                SetAvatar(avatarPath);
            }
        }

        private void RemoveAvatar_Click(object sender, RoutedEventArgs e)
        {
            avatarPath = null;
            SetAvatar(defaultAvatarPath);
        }

        private void SetAvatar(string path)
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                AvatarImage.Source = image;
            }
            catch
            {
                MessageBox.Show("Не вдалося завантажити аватар.");
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            user.Login = LoginBox.Text;
            user.Email = EmailBox.Text;
            user.FirstName = FirstNameBox.Text;
            user.LastName = LastNameBox.Text;
            user.Country = CountryBox.Text;
            user.City = CityBox.Text;
            user.BirthDate = BirthDatePicker.SelectedDate;
            user.Description = DescriptionBox.Text;
            user.AvatarPath = avatarPath;

            MessageBox.Show("Зміни збережено.");
            this.Close();
        }
    }
}
