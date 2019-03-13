using MineSweeper.DesktopClient.Controller;
using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Service.DataContracts;
using MineSweeper.Service.DataContracts.Faults;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MineSweeper.DesktopClient
{
    public partial class GameWindow : Window
    {
        private ClientGameController gameController;
        private List<FieldButton> fieldButtonPool;
        private List<FieldButton> currentFieldButtons;
        private bool isFirstUpdate;
        private int mineCount;
        private GameType gameType;
        public static Dictionary<int, SolidColorBrush> PlayerColors = new Dictionary<int, SolidColorBrush>();

        public delegate void sessionInvalidEventRaiser();
        public event sessionInvalidEventRaiser OnSessionInvalid;

        public GameWindow(ClientAccountController accountController, GameType gameType)
        {
            this.gameType = gameType;
            InitializeComponent();
            GenerateFieldButtons();
            currentFieldButtons = new List<FieldButton>();
            isFirstUpdate = true;
            InstantiateControllerAsync(accountController);
            Closing += GameWindow_ClosingAsync;
        }

        private async void GameWindow_ClosingAsync(object sender, System.ComponentModel.CancelEventArgs e)
        {
            await gameController.DisconnectAsync();
        }

        private async void InstantiateControllerAsync(ClientAccountController accountController)
        {
            gameController = new ClientGameController(accountController);
            gameController.OnGameUpdated += UpdateGUI;
            gameController.OnGameEnded += GameController_OnGameEnded;
            gameController.OnNewConsoleMessage += GameController_OnNewConsoleMessage;
            try
            {
                await gameController.JoinNewGameAsync(gameType);
            }
            catch (FaultException<InvalidSessionFault> f)
            {
                InvalidSessionErrorMessage(f);
            }
            catch (FaultException<DBFault> f)
            {
                DBErrorMessage(f);
            }
            catch (FaultException<UnclassifiedFault> f)
            {
                UnclassifiedFaultMessage(f);
            }
            catch (Exception ex)
            {
                ExceptionMessage(ex);
            }
        }

        private void DBErrorMessage(FaultException<DBFault> f)
        {
            MessageBox.Show("DB Error: " + f.Detail.Message);
            ErrorClose();
        }

        private void GameController_OnNewConsoleMessage(string message)
        {
            consoleStack.Children.Add(new TextBlock() { Text = message });
            console.ScrollToBottom();
        }

        private void GameController_OnGameEnded()
        {
            foreach (FieldButton fieldButton in currentFieldButtons)
            {
                fieldButton.ShowMine();
            }
            MessageBox.Show("GAME OVER");
        }

        private void GenerateFieldButtons()
        {
            fieldButtonPool = new List<FieldButton>();
            for (int i = 0; i < 500; i++)
            {
                FieldButton fieldButton = new FieldButton();
                fieldButton.Click += FieldButton_ClickAsync;
                fieldButton.MouseRightButtonDown += FieldButton_MouseRightButtonDown;
                fieldButtonPool.Add(fieldButton);
            }
        }

        private void FieldButton_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FieldButton fieldButton = sender as FieldButton;
            fieldButton.IsFlag = !fieldButton.IsFlag;
            fieldButton.UpdateButton();
            if (fieldButton.IsFlag)
            {
                mineCount--;
            }
            else
            {
                mineCount++;
            }
            _mineCounterLabel.Content = mineCount;
        }

        private async void FieldButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            FieldButton fieldButton = sender as FieldButton;
            if (!fieldButton.IsFlag)
            {
                try
                {
                    await gameController.TryActivateFieldAsync(fieldButton.Field);
                }
                catch (FaultException<InvalidSessionFault> f)
                {
                    InvalidSessionErrorMessage(f);
                }
                catch (FaultException<DBFault> f)
                {
                    DBErrorMessage(f);
                }
                catch (FaultException<InvalidIFieldActivationFault> f)
                {
                    InvalidIFieldActivationFaultMessage(f);
                }
                catch (FaultException<UnclassifiedFault> f)
                {
                    UnclassifiedFaultMessage(f);
                }
                catch (Exception ex)
                {
                    ExceptionMessage(ex);
                }
            }
        }

        public void UpdateGUI(GameData gameData)
        {
            if (isFirstUpdate)
            {
                FirstUpdate(gameData);
            }
            else
            {
                foreach (PlayerData opponentPlayer in gameData.OpponentPlayers)
                {
                    AddPlayerColor(opponentPlayer.Id);
                }
                for (int x = 0; x < gameData.Width; x++)
                {
                    for (int y = 0; y < gameData.Height; y++)
                    {
                        FieldData fieldData = gameData.Minefield[x, y];
                        FieldButton fieldButton = currentFieldButtons[x * gameData.Height + y];
                        fieldButton.Field = gameData.Minefield[x, y];
                        fieldButton.UpdateButton();
                        if (fieldData.PressedByPlayer != null && !fieldData.IsMine)
                        {
                            PlayerData playerData = gameData.OpponentPlayers.Find(player => player.Id == fieldData.PressedByPlayer.Id);
                            if (playerData == null)
                            {
                                playerData = gameData.Player;
                            }
                            playerData.Points++;
                        }
                    }
                }
            }
            List<PlayerData> allPlayers = new List<PlayerData>() { gameData.Player };
            allPlayers.AddRange(gameData.OpponentPlayers);
            allPlayers.Sort(new Comparison<PlayerData>(ComparePlayerData));
            scoreBoard.Children.Clear();
            int count = 1;
            foreach (PlayerData player in allPlayers)
            {
                TextBlock score = new TextBlock();
                string scoreString = count + ". " + player.Username + " (" + player.Points + ")";
                count++;
                if (!player.IsAlive)
                {
                    scoreString += " DEAD";
                }
                score.Text = scoreString;
                if (PlayerColors.TryGetValue(player.Id, out SolidColorBrush color))
                {
                    score.Background = color;
                }
                scoreBoard.Children.Add(score);
            }
        }

        private int ComparePlayerData(PlayerData player1, PlayerData player2)
        {
            return player2.Points - player1.Points;
        }

        private void FirstUpdate(GameData gameData)
        {
            AddPlayerColor(gameData.Player.Id);
            mineCount = gameData.MineAmount;
            _mineCounterLabel.Content = mineCount;
            SetGridDimensions(gameData);
            for (int x = 0; x < gameData.Width; x++)
            {
                ColumnDefinition column = new ColumnDefinition();
                buttonGrid.ColumnDefinitions.Add(column);
            }
            for (int y = 0; y < gameData.Height; y++)
            {
                RowDefinition row = new RowDefinition();
                buttonGrid.RowDefinitions.Add(row);
            }

            for (int x = 0; x < gameData.Width; x++)
            {
                for (int y = 0; y < gameData.Height; y++)
                {
                    FieldButton fieldButton = fieldButtonPool[x * gameData.Height + y];
                    fieldButton.Field = gameData.Minefield[x, y];
                    fieldButton.UpdateButton();
                    Grid.SetColumn(fieldButton, x);
                    Grid.SetRow(fieldButton, y);
                    buttonGrid.Children.Add(fieldButton);
                    currentFieldButtons.Add(fieldButton);
                }
            }
            isFirstUpdate = false;
            SetWindowAndComponentSize();
        }

        private void SetWindowAndComponentSize()
        {
            Width = buttonGrid.Width + scoreBoard.Width + 70;
            Height = buttonGrid.Height + console.Height + readyButton.Height + 70 + scoreBoard.Height;
        }

        private void SetGridDimensions(GameData gameData)
        {
            buttonGrid.Width = FieldButton.ButtonDimensions * gameData.Width;
            buttonGrid.Height = FieldButton.ButtonDimensions * gameData.Height;
        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                gameController.Ready();
                readyButton.IsEnabled = false;
            }
            catch (FaultException<DBFault> f)
            {
                DBErrorMessage(f);
            }
            catch (FaultException<UnclassifiedFault> f)
            {
                UnclassifiedFaultMessage(f);
            }
            catch (Exception ex)
            {
                ExceptionMessage(ex);
            }
        }


        private void InvalidSessionErrorMessage(FaultException<InvalidSessionFault> f)
        {
            MessageBox.Show(f.Detail.Message + "\nPlease log in again");
            ErrorClose();
        }

        private void InvalidIFieldActivationFaultMessage(FaultException<InvalidIFieldActivationFault> f)
        {
            MessageBox.Show("Invalid Field Error: " + f.Detail.Message);
        }

        private void ExceptionMessage(Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
            ErrorClose();
        }

        private void UnclassifiedFaultMessage(FaultException<UnclassifiedFault> f)
        {
            MessageBox.Show("Unclassified Error: " + f.Detail.Message);
            ErrorClose();
        }

        private void ErrorClose()
        {
            MessageBox.Show("Window will now close");
            Close();
        }

        public static void AddPlayerColor(int accountId)
        {
            if (!PlayerColors.ContainsKey(accountId))
            {
                SolidColorBrush color = null;
                switch (PlayerColors.Count)
                {
                    case 0:
                        color = new SolidColorBrush(Brushes.Green.Color);
                        break;
                    case 1:
                        color = new SolidColorBrush(Brushes.Blue.Color);
                        break;
                    case 2:
                        color = new SolidColorBrush(Brushes.Red.Color);
                        break;
                    case 3:
                        color = new SolidColorBrush(Brushes.Yellow.Color);
                        break;
                    case 4:
                        color = new SolidColorBrush(Brushes.Purple.Color);
                        break;
                    case 5:
                        color = new SolidColorBrush(Brushes.Orange.Color);
                        break;
                    default:
                        color = new SolidColorBrush(Brushes.Black.Color);
                        break;
                }
                color.Opacity = 0.5;
                PlayerColors.Add(accountId, color);
            }
        }
    }
}
