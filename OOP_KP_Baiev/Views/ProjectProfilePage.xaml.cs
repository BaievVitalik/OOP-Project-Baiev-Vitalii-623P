using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;

namespace OOP_KP_Baiev.Views
{
    public partial class ProjectProfilePage : Page
    {
        private readonly Frame _frame;
        private Project _project;
        private User _owner;

        public ProjectProfilePage(Guid projectId, Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            LoadProject(projectId);
        }

        private void LoadProject(Guid projectId)
        {
            _project = ProjectManager.Projects.FirstOrDefault(p => p.Id == projectId);
            if (_project == null)
            {
                MessageBox.Show("Проєкт не знайдено.");
                _frame.GoBack();
                return;
            }

            _owner = AccountManager.Users.FirstOrDefault(u => u.Login == _project.OwnerLogin);
            LoadProjectInfo();
        }

        private void LoadProjectInfo()
        {
            TitleText.Text = _project.Title ?? "Без назви";
            OwnerLoginText.Text = _project.OwnerLogin ?? "Невідомо";
            PriceText.Text = $"{_project.Price:C}";
            StatusText.Text = _project.Status.ToString();
            DescriptionText.Text = string.IsNullOrWhiteSpace(_project.Description) ? "Опис відсутній" : _project.Description;

            var currentUser = Application.Current.Properties["CurrentUser"] as User;
            if (currentUser == null) return;

            bool isOwner = currentUser is Customer && currentUser.Login == _project.OwnerLogin;

            ProjectDetailsPanel.Visibility = isOwner ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = isOwner && _project.Status == ProjectStatus.Active ? Visibility.Visible : Visibility.Collapsed;
            EditButton.Visibility = isOwner && _project.Status == ProjectStatus.Active ? Visibility.Visible : Visibility.Collapsed;

            if (isOwner && _project.Status == ProjectStatus.Active)
            {
                ShowFreelancerResponses();
                FreelancerListLabel.Visibility = Visibility.Visible;
                FreelancerCardsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                FreelancerListLabel.Visibility = Visibility.Collapsed;
                FreelancerCardsPanel.Visibility = Visibility.Collapsed;
            }

            if (_project.Status == ProjectStatus.Active &&
                currentUser is Freelancer freelancer &&
                !_project.RespondedFreelancers.Contains(freelancer.Id))
            {
                RespondButton.Visibility = Visibility.Visible;
            }
            else
            {
                RespondButton.Visibility = Visibility.Collapsed;
            }

            MarkAsDoneButton.Visibility = Visibility.Collapsed;
            PendingButtonsPanel.Visibility = Visibility.Collapsed;

            if (_project.Status == ProjectStatus.InProgress &&
                currentUser is Freelancer f &&
                f.Id == _project.FreelancerId)
            {
                MarkAsDoneButton.Visibility = Visibility.Visible;
            }
            else if (_project.Status == ProjectStatus.Pending &&
                     currentUser is Customer c &&
                     c.Login == _project.OwnerLogin)
            {
                PendingButtonsPanel.Visibility = Visibility.Visible;
            }
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            _frame?.Navigate(new MainPage(_frame));
        }

        private void OwnerLoginText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var frame = Window.GetWindow(this)?.FindName("MainFrame") as Frame;
            if (_owner == null || frame == null) return;

            var currentUser = Application.Current.Properties["CurrentUser"] as User;
            frame.Navigate(currentUser?.Id == _owner.Id
                ? new AccountPage(frame)
                : new UserProfilePage(_owner, frame));
        }

        private void OwnerLoginText_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb) tb.TextDecorations = TextDecorations.Underline;
        }

        private void OwnerLoginText_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock tb) tb.TextDecorations = null;
        }

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = Application.Current.Properties["CurrentUser"] as Customer;
            if (currentUser == null || currentUser.Login != _project.OwnerLogin)
            {
                MessageBox.Show("Ви не можете видалити цей проєкт.");
                return;
            }

            currentUser.Balance += _project.Price;
            ProjectManager.Remove(_project.Id.ToString());
            AccountManager.SaveData();

            MessageBox.Show("Проєкт видалено. Кошти повернуто на баланс.");
            _frame.Navigate(new MainPage(_frame));
        }

        private void RespondToProject_Click(object sender, RoutedEventArgs e)
        {
            var freelancer = Application.Current.Properties["CurrentUser"] as Freelancer;
            if (freelancer == null || _project.RespondedFreelancers.Contains(freelancer.Id)) return;

            _project.RespondedFreelancers.Add(freelancer.Id);
            ProjectManager.Save();

            RespondButton.Visibility = Visibility.Collapsed;
        }

        private void ShowFreelancerResponses()
        {
            var freelancers = _project.RespondedFreelancers
                .Select(id => AccountManager.Users
                    .OfType<Freelancer>()
                    .FirstOrDefault(f => f.Id == id))
                .Where(f => f != null)
                .ToList();

            FreelancerCardsPanel.ItemsSource = freelancers;
        }

        private void FreelancerName_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.Tag is Freelancer freelancer)
            {
                _frame.Navigate(new UserProfilePage(freelancer, _frame));
            }
        }

        private void RejectFreelancer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Freelancer freelancer)
            {
                _project.RespondedFreelancers.Remove(freelancer.Id);
                ProjectManager.Save();

                _frame.Navigate(new ProjectProfilePage(_project.Id, _frame));
            }
        }

        private void AssignFreelancer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Freelancer freelancer)
            {
                _project.FreelancerId = freelancer.Id;
                _project.Status = ProjectStatus.InProgress;
                _project.RespondedFreelancers.Clear();

                ProjectManager.Save();

                _frame.Navigate(new MainPage(_frame));
            }
        }

        private void EditProject_Click(object sender, RoutedEventArgs e)
        {
            if (_project != null)
            {
                _frame.Navigate(new EditProjectPage(_project, _frame));
            }
            else
            {
                MessageBox.Show("Проєкт не знайдено.");
            }
        }

        private void MarkAsDone_Click(object sender, RoutedEventArgs e)
        {
            var freelancer = Application.Current.Properties["CurrentUser"] as Freelancer;
            if (freelancer == null || freelancer.Id != _project.FreelancerId) return;

            _project.Status = ProjectStatus.Pending;
            ProjectManager.Save();

            LoadProjectInfo();
        }

        private void AcceptWork_Click(object sender, RoutedEventArgs e)
        {
            var customer = Application.Current.Properties["CurrentUser"] as Customer;
            if (customer == null || customer.Login != _project.OwnerLogin) return;

            var freelancer = AccountManager.Users
                .OfType<Freelancer>()
                .FirstOrDefault(f => f.Id == _project.FreelancerId);
            if (freelancer == null)
            {
                MessageBox.Show("Фрилансера не знайдено.");
                return;
            }

            freelancer.Balance += _project.Price;
            _project.Status = ProjectStatus.Completed;
            ProjectManager.Save();
            AccountManager.SaveData();

            MessageBox.Show("Проєкт завершено. Оплату нараховано фрилансеру.");
            LoadProjectInfo();
        }

        private void ContinueWork_Click(object sender, RoutedEventArgs e)
        {
            var customer = Application.Current.Properties["CurrentUser"] as Customer;
            if (customer == null || customer.Login != _project.OwnerLogin) return;

            _project.Status = ProjectStatus.InProgress;
            ProjectManager.Save();

            LoadProjectInfo();
        }
    }
}
