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
        private readonly Dictionary<string, List<string>> _countryCityMap = new Dictionary<string, List<string>> {
        { "Україна", new List<string> { "Київ", "Харків", "Львів" } },
        { "Польща", new List<string> { "Варшава", "Краків", "Гданськ" } },
        { "Німеччина", new List<string> { "Берлін", "Мюнхен", "Гамбург" } }
            };

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
            DescriptionBox.Text = _userToEdit.Description ?? "";
            BirthDatePicker.SelectedDate = _userToEdit.BirthDate;
            CountryComboBox.ItemsSource = _countryCityMap.Keys;

            if (!string.IsNullOrEmpty(_userToEdit.Country) && _countryCityMap.ContainsKey(_userToEdit.Country))
            {
                CountryComboBox.SelectedItem = _userToEdit.Country;
                CityComboBox.ItemsSource = _countryCityMap[_userToEdit.Country];

                if (_countryCityMap[_userToEdit.Country].Contains(_userToEdit.City))
                {
                    CityComboBox.SelectedItem = _userToEdit.City;
                }
            }

            _initialAvatarPath = _userToEdit.AvatarPath; 
            LoadAvatarImage(_userToEdit.AvatarPath);
        }
        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CountryComboBox.SelectedItem is string selectedCountry && _countryCityMap.ContainsKey(selectedCountry))
            {
                CityComboBox.ItemsSource = _countryCityMap[selectedCountry];
                CityComboBox.SelectedIndex = 0;
            }
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
        private string Capitalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim().ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
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

            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || FirstNameBox.Text.Length < 2 || FirstNameBox.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Ім'я повинно містити щонайменше 2 символи і не містити цифр.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameBox.Text) || LastNameBox.Text.Length < 2 || LastNameBox.Text.Any(char.IsDigit))
            {
                MessageBox.Show("Прізвище повинно містити щонайменше 2 символи і не містити цифр.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (BirthDatePicker.SelectedDate.HasValue)
            {
                if (BirthDatePicker.SelectedDate.Value < new DateTime(1950, 1, 1) ||
                    BirthDatePicker.SelectedDate.Value > DateTime.Now.AddYears(-16))
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

            _userToEdit.FirstName = Capitalize(FirstNameBox.Text);
            _userToEdit.LastName = Capitalize(LastNameBox.Text);
            _userToEdit.Description = DescriptionBox.Text.Trim();
            _userToEdit.BirthDate = BirthDatePicker.SelectedDate;
            if (CountryComboBox.SelectedItem is string selectedCountry)
                _userToEdit.Country = selectedCountry;

            if (CityComboBox.SelectedItem is string selectedCity)
                _userToEdit.City = selectedCity;

            if (AvatarImage.Tag is string newAvatarPath &&
            !string.IsNullOrEmpty(newAvatarPath) &&
            newAvatarPath != _initialAvatarPath)
            {
                _userToEdit.AvatarPath = newAvatarPath;
            }

            string defaultAvatarPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "default_avatar.png");
            if (string.IsNullOrEmpty(_userToEdit.AvatarPath) || !File.Exists(_userToEdit.AvatarPath))
            {
                _userToEdit.AvatarPath = defaultAvatarPath;
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