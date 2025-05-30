using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class CreateProjectPage : Page
    {
        private readonly Frame _frame;

        public CreateProjectPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new MainPage(_frame)); 
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string description = DescriptionBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.");
                return;
            }

            var currentUser = App.Current.Properties["CurrentUser"] as User;
            if (currentUser == null) 
            {
                MessageBox.Show("Помилка: поточний користувач не визначений. Неможливо створити проект.");
                _frame.Navigate(new LoginPage(_frame));
                return;
            }


            var newProject = new Project
            {
                Title = title,
                Description = description,
                Status = ProjectStatus.Active, 
                OwnerLogin = currentUser.Login,
            };
            if (currentUser is Customer customer) 
            {
                newProject.CustomerId = customer.Id;
            }

            ProjectManager.Add(newProject);

            MessageBox.Show("Проєкт створено!");
            _frame.Navigate(new ProjectListPage(currentUser, _frame));

        }
    }
}