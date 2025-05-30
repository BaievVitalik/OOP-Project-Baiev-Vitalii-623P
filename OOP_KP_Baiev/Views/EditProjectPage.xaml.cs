using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class EditProjectPage : Page
    {
        private readonly Frame _frame;
        private readonly Project _project;

        public EditProjectPage(Project project, Frame frame)
        {
            InitializeComponent();
            _project = project;
            _frame = frame;

            LoadProjectData();
        }

        private void LoadProjectData()
        {
            TitleBox.Text = _project.Title;
            DescriptionBox.Text = _project.Description;
            PriceBox.Text = _project.Price.ToString(CultureInfo.InvariantCulture);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
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

            if (!decimal.TryParse(priceText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal newPrice))
            {
                MessageBox.Show("Введіть коректну числову ціну.");
                return;
            }

            if (newPrice < 200 || newPrice > 100000)
            {
                MessageBox.Show("Ціна повинна бути від 200 до 100 000.");
                return;
            }

            var currentUser = Application.Current.Properties["CurrentUser"] as Customer;
            if (currentUser == null)
            {
                MessageBox.Show("Не вдалося отримати інформацію про користувача.");
                return;
            }

            decimal oldPrice = _project.Price;
            decimal priceDifference = newPrice - oldPrice;

            if (priceDifference > 0 && currentUser.Balance < priceDifference)
            {
                MessageBox.Show("Недостатньо коштів для оновлення ціни проєкту.");
                return;
            }

            currentUser.Balance -= priceDifference;

            var existingProject = ProjectManager.Projects.FirstOrDefault(p => p.Id == _project.Id);
            if (existingProject != null)
            {
                existingProject.Title = title;
                existingProject.Description = description;
                existingProject.Price = newPrice;

                ProjectManager.Save();
                AccountManager.SaveData(); 
            }

            MessageBox.Show("Проєкт оновлено.");
            _frame.Navigate(new ProjectProfilePage(_project.Id, _frame));
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
