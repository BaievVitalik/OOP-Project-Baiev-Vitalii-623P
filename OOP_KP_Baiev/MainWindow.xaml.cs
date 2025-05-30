using OOP_KP_Baiev.Views;
using OOP_KP_Baiev.Services;
using System.Windows;



namespace OOP_KP_Baiev

{

    public partial class MainWindow : Window

    {

        public MainWindow()

        {

            InitializeComponent();
            AccountManager.LoadData();
            MainFrame.Navigate(new LoginPage(MainFrame));

        }

    }

}