using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

            List<Project> filteredProjects = allProjects
                .Where(p => p.Status != ProjectStatus.Completed && p.Status != ProjectStatus.Pending)
                .ToList();

            ProjectList.ItemsSource = null;
            ProjectList.ItemsSource = filteredProjects;
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CreateProjectPage(_frame));
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            _frame?.Navigate(new MainPage(_frame));
        }
        private void Title_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                tb.TextDecorations = TextDecorations.Underline;
            }
        }

        private void Title_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                tb.TextDecorations = null;
            }
        }
        private void Title_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Project project)
            {
                _frame.Navigate(new ProjectProfilePage(project.Id, _frame));
            }
        }
    }
}