using Microsoft.Win32;
using OOP_KP_Baiev.Services;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OOP_KP_Baiev.Views
{
    public partial class ProfileEditPage : Page
    {
        private readonly Frame _frame;
        private User _userToEdit; 
        private string _initialAvatarPath; 

        public ProfileEditPage(Frame frame) : this(frame, App.Current.Properties["CurrentUser"] as User)
        {
        }

        public ProfileEditPage(Frame frame, User user)
        {
            InitializeComponent();
            _frame = frame;
            _userToEdit = user;

            if (_userToEdit == null)
            {
                MessageBox.Show("Помилка: користувач для редагування не визначений.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadProfileData();
        }

        private void LoadProfileData()
        {
            FirstNameBox.Text = _userToEdit.FirstName ?? "";
            LastNameBox.Text = _userToEdit.LastName ?? "";
            CountryBox.Text = _userToEdit.Country ?? "";
            CityBox.Text = _userToEdit.City ?? "";
            DescriptionBox.Text = _userToEdit.Description ?? "";
            BirthDatePicker.SelectedDate = _userToEdit.BirthDate;

            _initialAvatarPath = _userToEdit.AvatarPath; 
            LoadAvatarImage(_userToEdit.AvatarPath);
        }

        private void LoadAvatarImage(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                try
                {
                    BitmapImage bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.UriSource = new Uri(path, UriKind.Absolute);
                    bi.EndInit();
                    AvatarImage.Source = bi;
                }
                catch
                {
                    LoadDefaultAvatar(); 
                }
            }
            else
            {
                LoadDefaultAvatar(); 
            }
        }

        private void LoadDefaultAvatar()
        {
            try
            {
                AvatarImage.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Avatars/default_avatar.png"));
            }
            catch
            {
                AvatarImage.Source = null;
            }
        }


        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Зображення (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|Всі файли (*.*)|*.*",
                Title = "Виберіть аватар"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.FileName;

                string avatarsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Avatars");

                if (!Directory.Exists(avatarsDir))
                    Directory.CreateDirectory(avatarsDir);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedPath)}";
                string destPath = Path.Combine(avatarsDir, fileName);

                try
                {
                    File.Copy(selectedPath, destPath, true);
                    _userToEdit.AvatarPath = destPath;
                    AvatarImage.Tag = destPath;
                    LoadAvatarImage(destPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося завантажити аватар: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_userToEdit == null) return;

            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || FirstNameBox.Text.Length < 2)
            {
                MessageBox.Show("Ім'я повинно містити щонайменше 2 символи.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameBox.Text) || LastNameBox.Text.Length < 2)
            {
                MessageBox.Show("Прізвище повинно містити щонайменше 2 символи.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BirthDatePicker.SelectedDate.HasValue)
            {
                if (BirthDatePicker.SelectedDate.Value < new DateTime(1930, 1, 1) ||
                    BirthDatePicker.SelectedDate.Value > DateTime.Now.AddYears(-5))
                {
                    MessageBox.Show("Вказана некоректна дата народження.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            else
            {
                MessageBox.Show("Будь ласка, вкажіть дату народження.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DescriptionBox.Text.Length > 1000)
            {
                MessageBox.Show("Опис не повинен перевищувати 1000 символів.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _userToEdit.FirstName = FirstNameBox.Text.Trim();
            _userToEdit.LastName = LastNameBox.Text.Trim();
            _userToEdit.Country = CountryBox.Text.Trim();
            _userToEdit.City = CityBox.Text.Trim();
            _userToEdit.Description = DescriptionBox.Text.Trim();
            _userToEdit.BirthDate = BirthDatePicker.SelectedDate;

            if (AvatarImage.Tag is string newAvatarPath &&
                !string.IsNullOrEmpty(newAvatarPath) &&
                newAvatarPath != _initialAvatarPath)
            {
                _userToEdit.AvatarPath = newAvatarPath;
            }

            if (!AccountManager.Users.Any(u => u.Id == _userToEdit.Id))
            {
                AccountManager.Users.Add(_userToEdit);
            }

            AccountManager.SaveData();

            MessageBox.Show("Профіль успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);

            App.Current.Properties["CurrentUser"] = _userToEdit;

            _frame?.Navigate(new AccountPage(_frame));
        }
    }
}