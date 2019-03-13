using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Model;

namespace MineSweeper.ServerLogic.Tests.GameLogicTests
{
    /// <summary>
    /// Summary description for GameTests
    /// </summary>
    [TestClass]
    public class GameTests
    {
        Game game;

        [TestInitialize]
        public void TestInitialize()
        {
            game = new Game();
            game.Player = new Account() { Id = 1 };
        }

        [TestMethod]
        public void TestProperties()
        {

            Assert.AreEqual(1, game.Player.Id);
            Assert.AreEqual(0, game.OpponentPlayers.Count);
            Assert.IsFalse(game.IsCompleted);
            Assert.IsTrue(game.Height > 0);
            Assert.IsTrue(game.Width > 0);
            Assert.IsNotNull(game.Minefield);
            Assert.IsFalse(game.IsStarted);
            Assert.IsTrue(game.MineAmount > 0);
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
    }
}
