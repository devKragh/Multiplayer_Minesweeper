using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeper.Model;
using MineSweeper.ServerLogic.DB;
using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Service.DataContracts;
using MineSweeper.Service.Proxy;
using MineSweeper.Service.ServiceContracts;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace MineSweeper.Service.Test
{
    /// <summary>
    /// Summary description for GameServiceTests
    /// </summary>
    [TestClass]
    public class GameServiceTests
    {

        private ServiceHost accountServiceHost;
        private ServiceHost gameServiceHost;
        private AccountProxy accountProxy;
        private GameDuplexProxy gameDuplexProxy;
        private GameServiceCallback gameServiceCallback;
        private IGameDB gameDB;
        private Guid guid;
        private Account account;
        private string accountUsername;
        private string accountPassword;
        private GameType gameType;

        [TestInitialize]
        public void TestInitialize()
        {
            try
            {
                //accountUsername = "testUsername";
                //accountPassword = "gameServiceTestPassword";
                //gameType = GameType.FreeForAll;

                accountServiceHost = new ServiceHost(typeof(AccountService));
                accountServiceHost.Open();
                accountProxy = new AccountProxy("accountServiceClientSide");
                //accountProxy.RegisterNewUser(accountUsername, accountPassword);
                //account = accountProxy.GetAccount(accountUsername);
                //guid = accountProxy.AccountLogin(accountUsername, accountPassword);

                gameServiceHost = new ServiceHost(typeof(GameService));
                gameServiceHost.Open();
                //gameServiceCallback = new GameServiceCallback();
                //gameDuplexProxy = new GameDuplexProxy(new InstanceContext(gameServiceCallback), "duplexGameService");

                //gameDB = new GameDB();
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Initialization Error:\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            try
            {
                //gameDB.DeleteGame(gameServiceCallback.GameData.GameId);
                //accountProxy.DeactivateAccount(account.Id, guid);
                //gameDuplexProxy.Close();
                accountProxy.Close();
                accountServiceHost.Close();
                gameServiceHost.Close();
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Cleanup Error:\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }

        [TestMethod]
        public void TestJoinNewGame()
        {
            try
            {
                Assert.IsNull(gameServiceCallback.GameData);

                gameDuplexProxy.JoinNewGame(account.Id, gameType, guid);

                Assert.IsNotNull(gameServiceCallback.GameData);
                Assert.AreEqual(account.Id, gameServiceCallback.GameData.Player.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Service Error:\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }

        [TestMethod]
        public void TestMultiThreadJoinNewGame()
        {
            int numberOfAccounts = 5;
            string baseUsername = "testUser";
            string basePassword = "testPassword";
            List<string> usernames = new List<string>();
            List<string> passwords = new List<string>();
            List<Account> accounts = new List<Account>();
            List<Guid> guids = new List<Guid>();
            List<GameServiceCallback> gameServiceCallbacks = new List<GameServiceCallback>();
            List<GameDuplexProxy> gameProxies = new List<GameDuplexProxy>();
            List<Thread> threads = new List<Thread>();
            try
            {
                for (int i = 0; i < numberOfAccounts; i++)
                {
                    usernames.Add(baseUsername + i);
                    passwords.Add(basePassword + i);
                    accountProxy.RegisterNewUser(usernames[i], passwords[i]);
                    accounts.Add(accountProxy.GetAccount(usernames[i]));
                    guids.Add(accountProxy.AccountLogin(usernames[i], passwords[i]));
                    gameServiceCallbacks.Add(new GameServiceCallback());
                    gameProxies.Add(new GameDuplexProxy(new InstanceContext(gameServiceCallbacks[i]), "duplexGameService"));
                    threads.Add(new Thread(() =>
                    {
                        gameProxies[i].JoinNewGameAsync(accounts[i].Id, GameType.FreeForAll, guids[i]);
                    }));
                }

                foreach (Thread thread in threads)
                {
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [TestMethod]
        public void TestMakeMove()
        {
            try
            {
                gameDuplexProxy.JoinNewGame(account.Id, gameType, guid);
                int x = 1;
                int y = 1;
                FieldData fieldData = gameServiceCallback.GameData.JaggedMinefield[y][x];

                Assert.IsFalse(fieldData.IsPressed);
                Assert.IsNull(fieldData.PressedByPlayer);
                gameDuplexProxy.Ready();

                gameDuplexProxy.MakeMove(x, y, guid, account.Id);

                fieldData = gameServiceCallback.GameData.JaggedMinefield[y][x];
                Assert.IsTrue(fieldData.IsPressed);
                Assert.AreEqual(fieldData.PressedByPlayer.Id, account.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Service Error:\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }
    }

    public class GameServiceCallback : IGameCallbackService
    {
        public GameData GameData;
        public bool IsEnded = false;
        public void ConsoleMessage(string message)
        {
        }

        public void EndGame()
        {
            IsEnded = true;
        }

        public void UpdateClient(GameData gameData)
        {
            GameData = gameData;
        }
    }
}
