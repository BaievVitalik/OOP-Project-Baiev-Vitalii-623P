using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;

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
            var filteredProjects = _allUserProjects
                .Where(p => p.Status == targetStatus)
                .ToList();

            ProjectList.ItemsSource = filteredProjects;

            EmptyProjectsPlaceholder.Visibility = filteredProjects.Any()
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void Active_Click(object sender, RoutedEventArgs e) => ShowProjectsByStatus(ProjectStatus.Active);
        private void Completed_Click(object sender, RoutedEventArgs e) => ShowProjectsByStatus(ProjectStatus.Completed);

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            if (_frame != null)
            {
                _frame.Navigate(new MainPage(_frame));
            }
        }
    }
}
