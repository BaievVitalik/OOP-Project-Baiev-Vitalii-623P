using OOP_KP_Baiev.Models;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class MainPage : Page
    {
        private readonly Frame _frame;

        public MainPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            CheckUserRole();
        }

        private void CheckUserRole()
        {
            var currentUser = App.Current.Properties["CurrentUser"] as User;
            AdminPanelButton.Visibility = currentUser is Admin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GoToAdminPanel_Click(object sender, RoutedEventArgs e)
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

        private void GoToMyProjects_Click(object sender, RoutedEventArgs e)
        {
            _frame?.Navigate(new MyProjectsPage(_frame));
        }

        private void GoToFreelancers_Click(object sender, RoutedEventArgs e)
        {
            _frame?.Navigate(new FreelancerListPage(_frame));
        }

        private void GoToProjects_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = App.Current.Properties["CurrentUser"] as User;
            if (currentUser != null)
            {
                _frame?.Navigate(new ProjectListPage(currentUser, _frame));
            }
            else
            {
                MessageBox.Show("Користувач не визначений. Будь ласка, увійдіть у систему ще раз.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToAccount_Click(object sender, RoutedEventArgs e)
        {
            _frame?.Navigate(new AccountPage(_frame));
        }
    }
}
