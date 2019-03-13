using MineSweeper.DesktopClient.Controller;
using MineSweeper.Service.DataContracts.Faults;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;

namespace MineSweeper.DesktopClient
{
    /// <summary>
    /// Interaction logic for RegisterUserWindow.xaml
    /// </summary>
    public partial class RegisterUserWindow : Window
    {
        private ClientAccountController clientAccountController;
        public RegisterUserWindow()
        {
            InitializeComponent();
            clientAccountController = new ClientAccountController();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password.Equals(confirmPasswordBox.Password))
            {
                try
                {
                    clientAccountController.RegisterUser(usernameTextBox.Text, passwordBox.Password);
                    Close();
                }
                catch (FaultException<UsernameFault> f)
                {
                    MessageBox.Show(f.Detail.Message);
                }
            }
            else
            {
                MessageBox.Show("Password do not match");
            }
        }

        private void ConfirmPasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RegisterButton_Click(sender, e);
            }
        }
    }
}
