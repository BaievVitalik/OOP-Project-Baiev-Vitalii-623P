using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Globalization;
using System.Windows;

namespace OOP_KP_Baiev.Views
{
    public partial class EditProjectWindow : Window
    {
        private readonly Project _project;
        private readonly AdminPanelPage _adminPanelPage;

        public EditProjectWindow(Project project, AdminPanelPage adminPanelPage)
        {
            InitializeComponent();
            _project = project;
            _adminPanelPage = adminPanelPage;

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
            if (string.IsNullOrWhiteSpace(TitleBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionBox.Text) ||
                !decimal.TryParse(PriceBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Перевірте правильність введених даних.");
                return;
            }

            var existingProject = ProjectManager.Projects.FirstOrDefault(p => p.Id == _project.Id);
            if (existingProject != null)
            {
                existingProject.Title = TitleBox.Text.Trim();
                existingProject.Description = DescriptionBox.Text.Trim();
                existingProject.Price = price;
                ProjectManager.Save();
            }

            MessageBox.Show("Проєкт оновлено.");
            _adminPanelPage.RefreshUsersList(); 
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
