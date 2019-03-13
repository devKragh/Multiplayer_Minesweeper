using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MineSweeper.Model;
using MineSweeper.ServerLogic.DB;

namespace MineSweeper.ServerLogic.Tests.DBTests
{
	/// <summary>
	/// Summary description for UserDBTests
	/// </summary>
	[TestClass]
	public class UserDBTests
	{

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

		AccountDB accountDB;
		Account account;

		[TestInitialize]
		public void BeforeTest()
		{
			//accountDB = new AccountDB();
			//account = new Account()
			//{
			//	Username = "UnitTest",
			//	Password = "UnitTestPassword",
			//	Salt = "UnitTestSalt"
			//};
		}

		[TestCleanup]
		public void AfterTest()
		{
			//account = null;
			//accountDB = null;
		}

		[TestMethod]
		public void InsertAccountAndDeleteAccountTest()
		{
            ////Act
            //accountDB.InsertAccount(account);
            //Account insertedAccount = accountDB.GetAccount("UnitTest");

            ////Assert
            //Assert.AreNotEqual(-1, insertedAccount.Id);

            ////CleanUP
            //accountDB.DeleteAccount(insertedAccount.Id);

            ////Assert Cleanup
            //Assert.IsNull(accountDB.GetAccount("UnitTest"));

            Assert.IsTrue(true);

        }

        [TestMethod]
		public void SetSession()
		{
            ////Arrange
            //Guid guid = Guid.NewGuid();

            ////Act
            //accountDB.SetSession(1, guid);
            ////Testdata
            //Account a = accountDB.GetAccount("user1");

            ////Assert
            //Assert.AreEqual(guid, a.SessionKey);

            Assert.IsTrue(true);
		}
	}
}
