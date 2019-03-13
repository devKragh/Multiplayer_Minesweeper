using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Service.DataContracts;
using MineSweeper.Service.Proxy;
using MineSweeper.Service.ServiceContracts;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace MineSweeper.DesktopClient.Controller
{
    public class ClientGameController : IGameCallbackService
    {
        public delegate void updateGameEventRaiser(GameData gameData);
        public event updateGameEventRaiser OnGameUpdated;

        public delegate void gameEndedEventRaiser();
        public event gameEndedEventRaiser OnGameEnded;

        public delegate void consoleEventRaiser(string message);
        public event consoleEventRaiser OnNewConsoleMessage;

        private GameDuplexProxy gameProxy;
        public ClientAccountController accountController;

        public ClientGameController(ClientAccountController accountController)
        {
            this.accountController = accountController;
            gameProxy = new GameDuplexProxy(new InstanceContext(this), "duplexGameService");
            gameProxy.Open();
        }

        public Task JoinNewGameAsync(GameType gameType)
        {
            return gameProxy.JoinNewGameAsync(accountController.Account.Id, gameType, accountController.Guid);
        }

        public async Task DisconnectAsync()
        {
            Task task = gameProxy.DisconnectAsync();
            await task.ContinueWith(a =>
            {
                gameProxy.Close();
            });
        }

        public Task TryActivateFieldAsync(FieldData field)
        {
            return gameProxy.MakeMoveAsync(field.X, field.Y, accountController.Guid, accountController.Account.Id);
        }

        public void UpdateClient(GameData gameData)
        {
            if (OnGameUpdated != null)
            {
                ConvertJaggedMinefield(gameData);
                OnGameUpdated(gameData);
            }
        }

        private void ConvertJaggedMinefield(GameData gameData)
        {
            gameData.Minefield = new FieldData[gameData.Width, gameData.Height];
            for (int y = 0; y < gameData.Height; y++)
            {
                for (int x = 0; x < gameData.Width; x++)
                {
                    gameData.Minefield[x, y] = gameData.JaggedMinefield[y][x];
                }
            }
        }

        public void EndGame()
        {
            OnGameEnded?.Invoke();
        }

        public void Ready()
        {
            gameProxy.ReadyAsync();
        }

        public void ConsoleMessage(string message)
        {
            OnNewConsoleMessage?.Invoke(message);
        }
    }
}
