using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class CreateProjectPage : Page
    {
        private readonly Frame _frame;
        private readonly Freelancer _freelancer;
        private readonly Customer _customer;

        public CreateProjectPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            _frame?.Navigate(new MainPage(_frame));
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleBox.Text.Trim();
            string description = DescriptionBox.Text.Trim();
            string priceText = PriceBox.Text.Trim();

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(priceText))
            {
                MessageBox.Show("Будь ласка, заповніть усі поля.");
                return;
            }

            if (title.Length < 4 || title.Length > 100)
            {
                MessageBox.Show("Назва проєкту повинна містити від 4 до 100 символів.");
                return;
            }

            if (description.Length > 1000)
            {
                MessageBox.Show("Опис проєкту не повинен перевищувати 1000 символів.");
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Введіть коректну числову ціну.");
                return;
            }

            if (price < 200 || price > 100000)
            {
                MessageBox.Show("Ціна повинна бути від 200 до 100 000.");
                return;
            }

            var currentUser = App.Current.Properties["CurrentUser"] as User;
            if (currentUser == null)
            {
                MessageBox.Show("Помилка: поточний користувач не визначений. Неможливо створити проєкт.");
                _frame.Navigate(new LoginPage(_frame));
                return;
            }

            if (currentUser is Customer customer)
            {
                if (customer.Balance < price)
                {
                    MessageBox.Show("Недостатньо коштів на балансі для створення проєкту.");
                    return;
                }

                var newProject = new Project
                {
                    Title = title,
                    Description = description,
                    Status = ProjectStatus.Active,
                    OwnerLogin = customer.Login,
                    Price = price,
                    CustomerId = customer.Id,
                    FreelancerId = null
                };

                customer.Balance -= price;
                ProjectManager.Add(newProject);
                AccountManager.SaveData();

                MessageBox.Show("Проєкт створено. Кошти знято з балансу.");
                _frame.Navigate(new MainPage( _frame));
            }
            else
            {
                MessageBox.Show("Тільки замовник може створювати проєкти.");
            }
        }
    }
}