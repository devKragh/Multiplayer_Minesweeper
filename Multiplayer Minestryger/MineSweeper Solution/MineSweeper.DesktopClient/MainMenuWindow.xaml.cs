using MineSweeper.DesktopClient.Controller;
using MineSweeper.ServerLogic.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MineSweeper.DesktopClient
{
    /// <summary>
    /// Interaction logic for MainMenuWindow.xaml
    /// </summary>
    public partial class MainMenuWindow : Window
    {
        private ClientAccountController accountController;
        public MainMenuWindow(ClientAccountController accountController)
        {
            InitializeComponent();
            this.accountController = accountController;
            UpdateWinLoss();
        }

        private void quickMatchButton_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(accountController, GameType.QuickMatch);
            gameWindow.Closed += GameWindow_Closed;
            gameWindow.Show();
        }

        private void GameWindow_Closed(object sender, EventArgs e)
        {
            UpdateWinLoss();
        }

        private void fFAbutton_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(accountController, GameType.FreeForAll);
            gameWindow.Closed += GameWindow_Closed;
            gameWindow.Show();
        }

        private void UpdateWinLoss()
        {
            usernameLabel.Content = accountController.Account.Username;
            winLostNumbersLabel.Content = accountController.WinCount() + "/" + accountController.LossCount();
        }
    }
}
