using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class ProjectListPage : Page
    {
        private readonly Frame _frame;
        private readonly User _user;

        public ProjectListPage(User user, Frame frame)
        {
            InitializeComponent();
            _user = user;
            _frame = frame;
            if (_user is Freelancer)
            {
                CreateButton.Visibility = Visibility.Collapsed;
            }

            LoadProjects();
        }

        private void LoadProjects()
        {
            ProjectManager.Load(); 
            List<Project> allProjects = ProjectManager.Projects;
            ProjectList.ItemsSource = null;            
            ProjectList.ItemsSource = allProjects;       
        }
        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CreateProjectPage(_frame));
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new MainPage(_frame));
        }
    }
}
