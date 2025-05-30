using OOP_KP_Baiev.Services;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media; 
using OOP_KP_Baiev.Models; 

namespace OOP_KP_Baiev.Views
{
    public partial class LoginPage : Page
    {
        private readonly Frame _frame;
        private const string UsernamePlaceholderText = "Введіть ваш логін";
        private const string PasswordPlaceholderText = "Введіть ваш пароль";

        public LoginPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            InitializePlaceholders();
        }

        private void InitializePlaceholders()
        {
            if (string.IsNullOrWhiteSpace(UsernameBox.Text))
            {
                UsernameBox.Text = UsernamePlaceholderText;
                UsernameBox.Foreground = Brushes.Gray;
                UsernameBox.Tag = UsernamePlaceholderText;
            }

            UpdatePasswordPlaceholderVisibility();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = (UsernameBox.Text == UsernamePlaceholderText) ? string.Empty : UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Будь ласка, введіть логін та пароль.", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = AuthService.Login(login, password);
            if (user != null)
            {
                App.Current.Properties["CurrentUser"] = user;

                if (user is Admin)
                {
                    var adminWindow = new Window
                    {
                        Title = "Панель адміністратора",
                        Width = 850,
                        Height = 650,
                        Content = new AdminPanelPage(_frame),
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        MinHeight = 500,
                        MinWidth = 700
                    };
                    adminWindow.Show();
                }

                if (_frame != null)
                {
                    while (_frame.CanGoBack)
                    {
                        _frame.RemoveBackEntry();
                    }
                    _frame.Navigate(new MainPage(_frame));
                }
            }
            else
            {
                MessageBox.Show("Невірний логін або пароль.", "Помилка входу", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            if (_frame != null)
            {
                _frame.Navigate(new RegisterPage(_frame));
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Foreground == Brushes.Gray)
            {
                if (textBox.Text == (textBox.Tag as string ?? UsernamePlaceholderText)) 
                {
                    textBox.Text = "";
                    textBox.Foreground = SystemColors.WindowTextBrush;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag as string ?? UsernamePlaceholderText; 
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePasswordPlaceholderVisibility();
        }

        private void UpdatePasswordPlaceholderVisibility()
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}