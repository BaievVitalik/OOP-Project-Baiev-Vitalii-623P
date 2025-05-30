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
            _frame.Navigate(new MainPage(_frame)); // MainPage должна принимать Frame
        }

        private void LoadFreelancers()
        {
            var freelancers = AccountManager.Users.OfType<Freelancer>().ToList();
            FreelancerList.ItemsSource = freelancers;
        }
    }
}