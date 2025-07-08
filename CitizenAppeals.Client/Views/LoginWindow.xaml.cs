using System.Windows;
using CitizenAppeals.Client.ViewModels;

namespace CitizenAppeals.Client.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(PasswordBox);
        }
    }
}