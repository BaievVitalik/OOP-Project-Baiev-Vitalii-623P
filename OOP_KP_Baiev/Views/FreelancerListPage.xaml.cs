using OOP_KP_Baiev.Models;
using OOP_KP_Baiev.Services;
using System.Windows;
using System.Windows.Controls;

namespace OOP_KP_Baiev.Views
{
    public partial class FreelancerListPage : Page
    {
        private readonly Frame _frame;

        public FreelancerListPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;
            LoadFreelancers();
        }

        private void GoToMain_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }

        private void LoadFreelancers()
        {
            string defaultPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Avatars", "default_avatar.png");

            var freelancers = AccountManager.Users.OfType<Freelancer>().ToList();

            foreach (var freelancer in freelancers)
            {
                if (string.IsNullOrWhiteSpace(freelancer.AvatarPath) || !System.IO.File.Exists(freelancer.AvatarPath))
                {
                    freelancer.AvatarPath = defaultPath;
                }
            }

            FreelancerList.ItemsSource = freelancers;
        }
        private void FreelancerName_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is Freelancer selectedFreelancer)
            {
                var frame = Window.GetWindow(this)?.FindName("MainFrame") as Frame;
                if (frame != null)
                {
                    var currentUser = App.Current.Properties["CurrentUser"] as User;
                    if (currentUser != null && currentUser.Id == selectedFreelancer.Id)
                    {
                        frame.Navigate(new AccountPage(frame));
                    }
                    else
                    {
                        frame.Navigate(new UserProfilePage(selectedFreelancer, frame));
                    }
                }
            }
        }
        private void FreelancerName_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                tb.TextDecorations = TextDecorations.Underline;
            }
        }

        private void FreelancerName_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is TextBlock tb)
            {
                tb.TextDecorations = null;
            }
        }
    }
}