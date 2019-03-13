using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeper.ServerLogic.GameLogic;

namespace MineSweeper.ServerLogic.Tests.GameLogicTests
{
    /// <summary>
    /// Summary description for FieldTests
    /// </summary>
    [TestClass]
    public class FieldTests
    {
        Field[,] minefield;
        Field field;
        Game game;


        [TestInitialize]
        public void TestInitialize()
        {

            game = new Game();
            minefield = game.Minefield;
            field = minefield.GetValue(0, 0) as Field;
            
        }

        [TestMethod]
        public void TestProperties()
        {

            Assert.IsFalse(field.IsMine);
            Assert.AreEqual(0, field.X);
            Assert.AreEqual(0, field.Y);
            Assert.AreEqual(field.PressedByPlayerId, -1);
            Assert.IsNull(field.TimePressed);
            Assert.IsFalse(field.IsPressed);
            Assert.IsNotNull(field.AdjacentFields);
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

        [TestMethod]
        public void TestAdjacentMines()
        {
            Field field1 = minefield.GetValue(0, 0) as Field;
            Field field2 = minefield.GetValue(1, 0) as Field;
            Field field3 = minefield.GetValue(0, 1) as Field;
            Field field4 = minefield.GetValue(1, 1) as Field;

            field2.IsMine = true;
            field3.IsMine = true;
            field4.IsMine = true;

            Assert.AreEqual(3, field.AdjacentMines());

        }
    }
}
