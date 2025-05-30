using System.Windows;
using System.Windows.Controls;
using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;

namespace OOP_KP_Baiev.Views
{
    public partial class AdminPanelPage : Page
    {
        private readonly Frame _frame;

        public AdminPanelPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            LoadData();
        }

        private void LoadData()
        {
            UsersList.ItemsSource = AccountManager.Users
                .Where(u => u is not Admin)
                .ToList();

            ProjectsList.ItemsSource = ProjectManager.Projects.ToList();
        }

        private void ShowUsers_Click(object sender, RoutedEventArgs e)
        {
            UsersPanel.Visibility = Visibility.Visible;
            ProjectsPanel.Visibility = Visibility.Collapsed;
        }

        private void ShowProjects_Click(object sender, RoutedEventArgs e)
        {
            ProjectsPanel.Visibility = Visibility.Visible;
            UsersPanel.Visibility = Visibility.Collapsed;
        }

        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid userId)
            {
                var user = AccountManager.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    var editWindow = new EditUserWindow(user);
                    editWindow.ShowDialog(); 
                    LoadData(); 
                }
            }
        }

        private void DeleteUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid userId)
            {
                AccountManager.RemoveUser(userId);
                LoadData();
            }
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid projectId)
            {
                ProjectManager.Remove(projectId.ToString());
                LoadData();
            }
        }
        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Guid projectId)
            {
                var project = ProjectManager.Projects.FirstOrDefault(p => p.Id == projectId); 

                if (project != null)
                {
                    var editWindow = new EditProjectWindow(project, this);
                    editWindow.ShowDialog();
                    LoadData();
                }
            }
        }

        public void RefreshUsersList()
        {
            LoadData();
        }
    }
}
