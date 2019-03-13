using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeper.Model;
using MineSweeper.ServerLogic.BusinessLogic;
using MineSweeper.ServerLogic.DB;
using MineSweeper.ServerLogic.GameLogic;
using System;

namespace MineSweeper.ServerLogic.Tests.GameLogicTests
{
    /// <summary>
    /// Summary description for GameControllerTests
    /// </summary>
    [TestClass]
    public class GameControllerTests
    {

        private IGameController gameController;
        private IAccountController accountController;
        private Account account;
        private string accountUsername;
        private string accountPassword;
        private GameType gameType;

        [TestInitialize]
        public void TestInitialize()
        {
            try
            {
                accountUsername = "testUsername";
                accountPassword = "gameControllerTestPassword";
                gameType = GameType.FreeForAll;
                accountController = new AccountController();
                accountController.RegisterUser(accountUsername, accountPassword);
                account = accountController.GetAccount(accountUsername);
                gameController = new GameController(account);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Initialization Error\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }

        [TestMethod]
        public void TestJoinNewGame()
        {
            Assert.AreEqual(gameController.Game.GameType, GameType.Unknown);
            Assert.AreEqual(gameController.Game.Id, -1);

            gameController.JoinNewGame(gameType);

            Assert.AreEqual(gameController.Game.GameType, gameType);
            Assert.AreNotEqual(gameController.Game.Id, -1);
        }

        [TestMethod]
        public void TestTryActivateField()
        {
            gameController.JoinNewGame(gameType);
            gameController.Ready();
            gameController.CheckAllPlayerReadiness();

            int x = 1;
            int y = 1;
            Field field = gameController.Game.Minefield[x, y];
            Assert.IsNull(field.TimePressed);
            Assert.IsFalse(field.IsPressed);
            Assert.AreEqual(field.PressedByPlayerId, -1);

            gameController.TryActivateField(x, y);
            gameController.Update();

            field = gameController.Game.Minefield[x, y];
            Assert.IsNotNull(field.TimePressed);
            Assert.IsTrue(field.IsPressed);
            Assert.AreEqual(field.PressedByPlayerId, account.Id);
        }

        [TestMethod]
        public void TestCheckAllPlayersReady()
        {
            gameController.JoinNewGame(gameType);
            Assert.IsFalse(gameController.AllPlayersReady);

            gameController.Ready();
            gameController.CheckAllPlayerReadiness();

            Assert.IsTrue(gameController.AllPlayersReady);
        }

        [TestCleanup]
        public void CleanUp()
        {
            try
            {
                Guid guid = accountController.VerifyPassword(accountUsername, accountPassword);
                accountController.DeactivateAccount(account.Id, guid);
                IGameDB gameDB = new GameDB();
                gameDB.DeleteGame(gameController.Game.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Cleanup Error\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }

        }

    }
}
