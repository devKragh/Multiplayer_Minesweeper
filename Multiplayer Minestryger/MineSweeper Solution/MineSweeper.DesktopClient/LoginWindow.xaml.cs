using MineSweeper.DesktopClient.Controller;
using MineSweeper.Service.DataContracts.Faults;
using NetFwTypeLib;
using System;
using System.ServiceModel;
using System.Windows;

namespace MineSweeper.DesktopClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ClientAccountController accountController;

        public LoginWindow()
        {
            openAccess();
            InitializeComponent();
            accountController = new ClientAccountController();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                accountController.Login(usernameTextBox.Text, passwordBox.Password);
                MainMenuWindow mainMenuWindow = new MainMenuWindow(accountController);
                mainMenuWindow.Show();
                Close();
            }
            catch (FaultException<InvalidUserCredentialsFault> fault)
            {
                MessageBox.Show(fault.Detail.Message);
                passwordBox.Password = "";
            }
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterUserWindow registerUserWindow = new RegisterUserWindow();
            registerUserWindow.Show();
        }

        private void openAccess()
        {
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            INetFwPolicy2 fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            var currentProfiles = fwPolicy2.CurrentProfileTypes;

            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Enabled = true;
            
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
            
            inboundRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;

            inboundRule.LocalPorts = "80";
            
            inboundRule.Name = "MultiplayerMineSweeper";
            
            inboundRule.Profiles = currentProfiles;

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            foreach (INetFwRule2 rule in firewallPolicy.Rules)
            {
                if (rule.Name.IndexOf(inboundRule.Name) != -1)
                {
                    firewallPolicy.Rules.Remove(inboundRule.Name);
                }
            }
            firewallPolicy.Rules.Add(inboundRule);
        }

        private void PasswordBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                loginButton_Click(sender, e);
            }
        }
    }

	
}
