using MineSweeper.Model;
using MineSweeper.ServerLogic.Exceptions;
using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Service.DataContracts;
using MineSweeper.Service.DataContracts.Faults;
using MineSweeper.Service.Proxy;
using MineSweeper.Service.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Threading;

namespace MineSweeper.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class GameService : ServiceContracts.IGameService
    {
        private IGameController gameController;
        private GameCallback gameCallback;
        private IGameCallbackService clientProxy;
        private Guid currentGuid = Guid.Empty;

        public void JoinNewGame(int accountId, GameType gameType, Guid guid)
        {
            AccountProxy accountProxy = new AccountProxy("accountService");
            if (accountProxy.ValidateSession(accountId, guid))
            {
                currentGuid = guid;
                try
                {
                    gameController = new GameController(accountProxy.GetAccountById(accountId));
                    gameController.JoinNewGame(gameType);
                    gameCallback = GameCallback.GetGameCallbackInstance(gameController.Game.Id);
                    clientProxy = OperationContext.Current.GetCallbackChannel<IGameCallbackService>();
                    gameCallback.AddCallback(this);
                    gameCallback.Update(!gameController.Game.IsStarted);
                }
                catch (DBException ex)
                {
                    clientProxy.EndGame();
                    throw new FaultException<DBFault>(new DBFault());
                }
                catch (SqlException ex)
                {
                    clientProxy.EndGame();
                    throw new FaultException<DBFault>(new DBFault());
                }
                catch (Exception ex)
                {
                    clientProxy.EndGame();
                    throw new FaultException<UnclassifiedFault>(new UnclassifiedFault(ex.Message), new FaultReason(ex.Message));
                }
            }
            else
            {
                clientProxy.EndGame();
                throw new FaultException<InvalidSessionFault>(new InvalidSessionFault());
            }
        }

        public void MakeMove(int x, int y, Guid guid, int accountId)
        {
            if (gameController.AllPlayersReady && gameController.Game.Player.IsAlive && !gameController.Game.IsCompleted)
            {
                if (guid.Equals(currentGuid))
                {
                    try
                    {
                        lock (gameController)
                        {
                            gameController.TryActivateField(x, y);
                        }
                        Console.WriteLine("user: " + gameController.Game.Player.Username + " activated point(x: " + x + ",y:" + y + ")");
                        gameCallback.Update(!gameController.Game.IsStarted);
                    }
                    catch (SqlException ex)
                    {
                        clientProxy.EndGame();
                        Console.WriteLine(ex.Message);
                        throw new FaultException<DBFault>(new DBFault());
                    }
                    catch (DBException ex)
                    {
                        clientProxy.EndGame();
                        Console.WriteLine(ex.Message);
                        throw new FaultException<DBFault>(new DBFault());
                    }
                    catch (InvalidFieldActivationException ex)
                    {
                        clientProxy.EndGame();
                        Console.WriteLine(ex.Message);
                        throw new FaultException<InvalidIFieldActivationFault>(new InvalidIFieldActivationFault(ex.Message));
                    }
                    catch (Exception ex)
                    {
                        clientProxy.EndGame();
                        Console.WriteLine(ex.Message);
                        throw new FaultException<UnclassifiedFault>(new UnclassifiedFault(ex.Message));
                    }
                }
                else
                {
                    clientProxy.EndGame();
                    throw new FaultException<InvalidSessionFault>(new InvalidSessionFault());
                }
            }
        }

        public void SendConsoleMessage(string message)
        {
            Console.WriteLine(message);
            clientProxy.ConsoleMessage(message);
        }

        public void Ready()
        {
            try
            {
                gameController.Ready();
                gameCallback.SendConsoleMessageToAll("Player: " + gameController.Game.Player.Username + " is ready", !gameController.Game.IsStarted);
                gameCallback.CheckAllPlayersReady(!gameController.Game.IsStarted);
            }
            catch (SqlException ex)
            {
                throw new FaultException<DBFault>(new DBFault());
            }
            catch (DBException ex)
            {
                throw new FaultException<DBFault>(new DBFault());
            }
            catch (Exception ex)
            {
                throw new FaultException<UnclassifiedFault>(new UnclassifiedFault(ex.Message));
            }
        }

        public void Update()
        {
            lock (gameController)
            {
                gameController.Update();
            }
            if (gameController.Game.IsCompleted)
            {
                clientProxy.UpdateClient(GetConvertedGameData(true));
                Console.WriteLine("user: " + gameController.Game.Player.Username + " updated from game id: " + gameController.Game.Id);
                clientProxy.EndGame();
                Console.WriteLine("game id: " + gameController.Game.Id + " ended");
            }
            else
            {
                clientProxy.UpdateClient(GetConvertedGameData(false));
                Console.WriteLine("user: " + gameController.Game.Player.Username + " updated from game id: " + gameController.Game.Id);

            }
        }

        public void CheckAllPlayerReadiness()
        {
            if (gameController.CheckAllPlayerReadiness())
            {
                clientProxy.ConsoleMessage(
                    "---------------------------------------------\n" +
                    "All players ready\n" +
                    "Start!\n" +
                    "---------------------------------------------");
            }
        }

        public void Disconnect()
        {
            gameCallback.Disconnect(this);
            gameController.UnjoinGame();
            gameCallback.SendConsoleMessageToAll("User : " + gameController.Game.Player.Username + " disconnected from game id : " + gameController.Game.Id, !gameController.Game.IsStarted);
        }

        private GameData GetConvertedGameData(bool includeMines)
        {
            return new GameData()
            {
                GameId = gameController.Game.Id,
                Player = GetConvertedPlayerData(gameController.Game.Player.Id),
                OpponentPlayers = GetConvertedOpponentPlayerData(),
                Height = gameController.Game.Height,
                Width = gameController.Game.Width,
                MineAmount = gameController.Game.MineAmount,
                JaggedMinefield = GetConvertedMinefield(includeMines)
            };
        }

        private List<PlayerData> GetConvertedOpponentPlayerData()
        {
            AccountProxy accountProxy = new AccountProxy("accountService");
            List<PlayerData> playerDatas = new List<PlayerData>();
            foreach (Account player in gameController.Game.OpponentPlayers)
            {
                Account account = accountProxy.GetAccountById(player.Id);
                playerDatas.Add(new PlayerData() { Id = account.Id, Username = account.Username, IsAlive = player.IsAlive, Points = player.Points });
            }
            return playerDatas;
        }

        private PlayerData GetConvertedPlayerData(int accountId)
        {
            PlayerData playerData = null;
            AccountProxy accountProxy = new AccountProxy("accountService");
            Account player = gameController.Game.OpponentPlayers.Find(x => x.Id == accountId);
            if (player == null && accountId == gameController.Game.Player.Id)
            {
                player = gameController.Game.Player;
            }
            if (player != null)
            {
                playerData = new PlayerData()
                {
                    Id = player.Id,
                    Username = player.Username,
                    IsAlive = player.IsAlive,
                    Points = player.Points
                };
            }
            return playerData;
        }

        private FieldData[][] GetConvertedMinefield(bool includeMines)
        {
            FieldData[][] minefield = new FieldData[gameController.Game.Height][];
            for (int y = 0; y < gameController.Game.Height; y++)
            {
                minefield[y] = new FieldData[gameController.Game.Width];
                for (int x = 0; x < gameController.Game.Width; x++)
                {
                    Field field = gameController.Game.Minefield[x, y];
                    FieldData fieldData = new FieldData();
                    fieldData.X = field.X;
                    fieldData.Y = field.Y;
                    fieldData.PressedByPlayer = GetConvertedPlayerData(field.PressedByPlayerId);
                    fieldData.IsPressed = field.IsPressed;
                    if (field.IsPressed)
                    {
                        fieldData.AdjacentMines = field.AdjacentMines();
                        fieldData.IsMine = field.IsMine;
                    }
                    if (includeMines)
                    {
                        fieldData.IsMine = field.IsMine;
                    }
                    minefield[field.Y][field.X] = fieldData;
                }
            }
            return minefield;
        }

        public int JoinWebGame(int accountId, GameType gameType, Guid guid)
        {
            JoinNewGame(accountId, gameType, guid);
            return gameController.Game.Id;
        }

        public void JoinActiveGame(int accountId, Guid guid)
        {
            throw new FaultException<NotImplementedFault>(new NotImplementedFault(""));
        }

    }
}
