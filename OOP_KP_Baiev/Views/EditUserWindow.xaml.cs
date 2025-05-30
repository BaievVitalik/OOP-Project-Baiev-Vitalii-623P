using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace OOP_KP_Baiev.Views
{
    public partial class EditUserWindow : Window
    {
        private User user;
        private string avatarPath;
        private readonly string defaultAvatarPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Avatars", "default_avatar.png");
        private readonly Dictionary<string, List<string>> _countryCityMap = new Dictionary<string, List<string>> {
            { "Україна", new List<string> { "Київ", "Харків", "Львів" } },
            { "Польща", new List<string> { "Варшава", "Краків", "Гданськ" } },
            { "Німеччина", new List<string> { "Берлін", "Мюнхен", "Гамбург" } }
        };

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
            BirthDatePicker.SelectedDate = user.BirthDate;
            CountryComboBox.ItemsSource = _countryCityMap.Keys;

            if (!string.IsNullOrEmpty(user.Country) && _countryCityMap.ContainsKey(user.Country))
            {
                CountryComboBox.SelectedItem = user.Country;
                CityComboBox.ItemsSource = _countryCityMap[user.Country];

                if (_countryCityMap[user.Country].Contains(user.City))
                {
                    CityComboBox.SelectedItem = user.City;
                }
            }


            avatarPath = user.AvatarPath ?? defaultAvatarPath;
            SetAvatar(avatarPath);
        }

        private void CountryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CountryComboBox.SelectedItem is string selectedCountry && _countryCityMap.ContainsKey(selectedCountry))
            {
                CityComboBox.ItemsSource = _countryCityMap[selectedCountry];
                CityComboBox.SelectedIndex = 0;
            }
        }

        private void UploadAvatar_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Зображення (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Виберіть аватар"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedPath = openFileDialog.FileName;
                string avatarsDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Avatars");

                if (!Directory.Exists(avatarsDir))
                    Directory.CreateDirectory(avatarsDir);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(selectedPath)}";
                string destPath = Path.Combine(avatarsDir, fileName);

                try
                {
                    File.Copy(selectedPath, destPath, true);
                    avatarPath = destPath;
                    SetAvatar(avatarPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося завантажити аватар: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void RemoveAvatar_Click(object sender, RoutedEventArgs e)
        {
            avatarPath = defaultAvatarPath;
            SetAvatar(avatarPath);
        }

        private void SetAvatar(string path)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(path, UriKind.Absolute);
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                AvatarImage.Source = image;
            }
            catch
            {
                MessageBox.Show("Не вдалося завантажити аватар.");
            }
        }

        private string Capitalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = input.Trim().ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Regex latinOnlyRegex = new Regex("^[a-zA-Z0-9@._]+$");

            if (!latinOnlyRegex.IsMatch(LoginBox.Text))
            {
                MessageBox.Show("Логін повинен містити лише латинські літери, цифри або символи '@', '.', '_'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (LoginBox.Text.StartsWith("_") || LoginBox.Text.Length < 4 || LoginBox.Text.Length > 15)
            {
                MessageBox.Show("Логін повинен містити від 4 до 15 символів і не починатися з символу '_'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!latinOnlyRegex.IsMatch(EmailBox.Text))
            {
                MessageBox.Show("Email повинен містити лише латинські літери, цифри або символи '@', '.', '_'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (EmailBox.Text.StartsWith("_") || EmailBox.Text.Length < 5 || EmailBox.Text.Length > 30)
            {
                MessageBox.Show("Пошта повинна містити від 5 до 30 символів і не починатися з символу '_'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EmailBox.Text.EndsWith("@gmail.com", System.StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email повинен закінчуватись на '@gmail.com'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (!EmailBox.Text.EndsWith("@gmail.com", System.StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email повинен закінчуватись на '@gmail.com'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
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

            if (!BirthDatePicker.SelectedDate.HasValue ||
                BirthDatePicker.SelectedDate.Value < new DateTime(1950, 1, 1) ||
                BirthDatePicker.SelectedDate.Value > DateTime.Now.AddYears(-16))
            {
                MessageBox.Show("Вказана некоректна дата народження.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DescriptionBox.Text.Length > 1000)
            {
                MessageBox.Show("Опис не повинен перевищувати 1000 символів.", "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            user.FirstName = Capitalize(FirstNameBox.Text);
            user.LastName = Capitalize(LastNameBox.Text);
            user.BirthDate = BirthDatePicker.SelectedDate;
            user.Description = DescriptionBox.Text.Trim();
            user.AvatarPath = avatarPath;
            if (CountryComboBox.SelectedItem is string selectedCountry)
                user.Country = selectedCountry;

            if (CityComboBox.SelectedItem is string selectedCity)
                user.City = selectedCity;

            MessageBox.Show("Зміни збережено.");
            this.Close();
        }
    }
}
