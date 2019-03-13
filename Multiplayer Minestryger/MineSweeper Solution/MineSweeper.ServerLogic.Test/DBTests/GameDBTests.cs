using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeper.Model;
using MineSweeper.ServerLogic.BusinessLogic;
using MineSweeper.ServerLogic.DB;
using MineSweeper.ServerLogic.GameLogic;
using System;

namespace MineSweeper.ServerLogic.Tests.DBTests
{
    [TestClass]
    public class GameDBTests
    {
        private Game game;
        private Account account;
        private string accountUsername;
        private string accountPassword;
        private IAccountController accountController;
        private IGameDB gameDB;

        [TestInitialize]
        public void TestInitialize()
        {
            try
            {
                accountUsername = "testUser";
                accountPassword = "testPassword";
                accountController = new AccountController();
                accountController.RegisterUser(accountUsername, accountPassword);
                account = accountController.GetAccount(accountUsername);
                game = new Game();
                game.Player = account;
                game.GameType = GameType.FreeForAll;
                gameDB = new GameDB();
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Initialization Error\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }


        [TestMethod]
        public void TestJoinGame()
        {
            Assert.AreEqual(game.Id, -1);

            gameDB.JoinGame(game);

            Assert.AreNotEqual(-1, game.Id);

        }

        [TestMethod]
        public void TestMakeMove()
        {
            int fieldX = 1;
            int fieldY = 1;
            gameDB.JoinGame(game);

            Field field = game.Minefield[fieldX, fieldY];
            Assert.IsNull(field.TimePressed);
            Assert.IsFalse(field.IsPressed);
            Assert.AreEqual(field.PressedByPlayerId, -1);
            gameDB.StartGame(game);

            field.IsPressed = true;
            field.PressedByPlayerId = account.Id;

            gameDB.MakeMove(game, new System.Collections.Generic.List<Field>() { field });

            gameDB.UpdateGameObject(game);

            field = game.Minefield[fieldX, fieldY];
            Assert.IsNotNull(field.TimePressed);
            Assert.IsTrue(field.IsPressed);
            Assert.AreEqual(field.PressedByPlayerId, account.Id);

        }

        [TestMethod]
        public void TestCheckAllPlayerReadiness()
        {
            gameDB.JoinGame(game);
            Assert.IsFalse(gameDB.CheckAllPlayerReadiness());
            gameDB.SetPlayerReady(game);
            gameDB.UpdateGameObject(game);
            Assert.IsTrue(gameDB.CheckAllPlayerReadiness());

        }

        [TestMethod]
        public void TestEndGame()
        {
            gameDB.JoinGame(game);
            Assert.IsFalse(game.IsCompleted);

            gameDB.EndGame(game);
            gameDB.UpdateGameObject(game);

            Assert.IsTrue(game.IsCompleted);

        }

        [TestMethod]
        public void TestStartGame()
        {
            gameDB.JoinGame(game);
            Assert.IsFalse(game.IsStarted);

            gameDB.StartGame(game);
            gameDB.UpdateGameObject(game);

            Assert.IsTrue(game.IsStarted);

        }

        [TestMethod]
        public void TestSetPlayerDead()
        {
            gameDB.JoinGame(game);
            Assert.IsTrue(game.Player.IsAlive);

            gameDB.SetPlayerDead(game);
            gameDB.UpdateGameObject(game);

            Assert.IsFalse(game.Player.IsAlive);

        }

        [TestCleanup]
        public void StandardCleanUp()
        {
            try
            {
                accountController.DeactivateAccount(account.Id, accountController.VerifyPassword(accountUsername, accountPassword));
                gameDB.DeleteGame(game.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(string.Format("Cleanup Error\n{0}\n{1}\n{2}", ex.GetType(), ex.Message, ex.StackTrace));
            }
        }
    }
}
