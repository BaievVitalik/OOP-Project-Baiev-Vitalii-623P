using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OOP_KP_Baiev.Views
{
    public partial class MyProjectsPage : Page
    {
        private readonly Frame _frame;
        private Guid _currentUserId;
        private List<Project> _allUserProjects;

        public MyProjectsPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;

            var currentUser = App.Current.Properties["CurrentUser"] as User;
            if (currentUser != null)
            {
                _currentUserId = currentUser.Id;

                ProjectManager.Load();

                if (currentUser is Customer || currentUser is Admin)
                {
                    _allUserProjects = ProjectManager.Projects
                        .Where(p => p.CustomerId == _currentUserId)
                        .ToList();
                }
                else if (currentUser is Freelancer freelancer)
                {
                    _allUserProjects = ProjectManager.Projects
                        .Where(p => p.FreelancerId == _currentUserId)
                        .ToList();
                }
                else
                {
                    _allUserProjects = new List<Project>();
                }
            }
            else
            {
                MessageBox.Show("Помилка: поточний користувач не знайдений. Повернення на сторінку входу.");
                if (_frame != null && _frame.CanGoBack)
                {
                    _frame.GoBack();
                }
                else if (_frame != null)
                {
                    _frame.Navigate(new LoginPage(_frame));
                }
                return;
            }

            ShowProjectsByStatus(ProjectStatus.Active);
        }

        private void ShowProjectsByStatus(ProjectStatus targetStatus)
        {
            List<Project> filteredProjects;

            if (targetStatus == ProjectStatus.Active)
            {
                filteredProjects = _allUserProjects
                    .Where(p => p.Status == ProjectStatus.Active ||
                                p.Status == ProjectStatus.Pending ||
                                p.Status == ProjectStatus.InProgress)
                    .ToList();
            }
            else
            {
                filteredProjects = _allUserProjects
                    .Where(p => p.Status == targetStatus)
                    .ToList();
            }

            ProjectList.ItemsSource = filteredProjects;

            EmptyProjectsPlaceholder.Visibility = filteredProjects.Any()
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Active_Click(object sender,  RoutedEventArgs e) => ShowProjectsByStatus(ProjectStatus.Active);
        private void Completed_Click(object sender, RoutedEventArgs e) => ShowProjectsByStatus(ProjectStatus.Completed);
        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            if (_frame != null)
            {
                _frame.GoBack();
            }
        }
        private void Title_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb)
                tb.TextDecorations = TextDecorations.Underline;
        }

        private void Title_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb)
                tb.TextDecorations = null;
        }

        private void Title_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Project project)
            {
                _frame.Navigate(new ProjectProfilePage(project.Id, _frame));
            }
        }
        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = App.Current.Properties["CurrentUser"] as User;
            if (currentUser == null)
            {
                MessageBox.Show("Користувач не знайдений.");
                return;
            }

            if (currentUser is Customer)
            {
                NavigationService?.Navigate(new CreateProjectPage(_frame));
            }
            else if (currentUser is Freelancer)
            {
                NavigationService?.Navigate(new ProjectListPage(currentUser, _frame));
            }
        }
    }
}
