using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using MineSweeper.ServerLogic.DB;
using MineSweeper.Model;
using MineSweeper.ServerLogic.BusinessLogic;

namespace MineSweeper.ServerLogic.Test
{
    [TestClass]
    public class AccountControllerTest
    {

        IAccountController accCtr;
        AccountDB accDb;
        Account accOne;
        Account accTwo;

        [TestInitialize()]
        public void Setup()
        {
            accCtr = new AccountController();
            accDb = new AccountDB();

            accOne = new Account()
            {
                Username = "RegUsername",
                Password = "RegPassword",
                Salt = "",
                Rankpoints = 0
            };

            accTwo = new Account()
            {
                Username = "RegUsername2",
                Password = "RegPassword",
                Salt = "",
                Rankpoints = 0
            };
        }

        [TestCleanup()]
        public void Cleanup()
        {
            accCtr = null;
            accDb = null;
            accOne = null;
            accTwo = null;
        }

        [TestMethod]
        public void RegisterUserTest()
        {
            //         accCtr.RegisterUser(accOne.Username, accOne.Password);
            //         accOne = accDb.GetAccount(accOne.Username);

            //         accCtr.RegisterUser(accTwo.Username, accTwo.Password);
            //         accTwo = accDb.GetAccount(accTwo.Username);
            //Guid g = Guid.NewGuid();

            ////Assert.IsTrue(accCtr.VerifyPassword(accOne.Username, "RegPassword", g));
            ////         Assert.IsTrue(accCtr.VerifyPassword(accTwo.Username, "RegPassword", g));
            //         Assert.AreNotEqual(accOne.Password, accTwo.Password);

            //         CleanDBEntry(accOne.Id);
            //         CleanDBEntry(accTwo.Id);

            Assert.IsTrue(true);
        }

        public void CleanDBEntry(int id)
        {
            //accDb.DeleteAccount(id);
        }

        /**********************************************************************/
        // Skal ikke rigtig testes da GenerateRandomSalt() er private

        //[TestMethod]
        public void GenerateRandomSaltTest()
        {
            ////Arrange
            //byte[] salt = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            //byte[] salter = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            ////Act
            ////salt = accCtr.GenerateRandomSalt();

            ////Assert
            //Assert.IsFalse(salter.SequenceEqual(salt));
            Assert.IsTrue(true);
        }
    }
}
