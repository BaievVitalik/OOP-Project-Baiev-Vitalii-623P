using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class RegisterPage : Page
    {
        private readonly Frame _frame;

        public RegisterPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = UsernameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (login.StartsWith("_") || login.Length < 4 || login.Length > 15)
            {
                MessageBox.Show("Логін повинен містити від 4 до 15 символів і не починатися з символу '_'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.StartsWith("_") || password.Length < 5 || password.Length > 30)
            {
                MessageBox.Show("Пароль повинен містити від 5 до 30 символів і не починатися з символу '_'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!email.EndsWith("@gmail.com", System.StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email повинен закінчуватись на '@gmail.com'.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (AccountManager.Users.Any(u => u.Login == login || u.Email == email))
            {
                MessageBox.Show("Користувач з таким логіном або email вже існує.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            User newUser;

            if (CustomerRadio.IsChecked == true)
                newUser = new Customer();
            else if (FreelancerRadio.IsChecked == true)
                newUser = new Freelancer();
            else
            {
                MessageBox.Show("Оберіть роль користувача.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            newUser.Login = login;
            newUser.Email = email;
            newUser.Password = password;
            App.Current.Properties["CurrentUser"] = newUser;
            newUser.AvatarPath = "";

            _frame.Navigate(new ProfileEditPage(_frame, newUser));
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new LoginPage(_frame));
        }
    }
}