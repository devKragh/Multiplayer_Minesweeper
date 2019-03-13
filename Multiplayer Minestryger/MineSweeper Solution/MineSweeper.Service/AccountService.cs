using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MineSweeper.Model;
using MineSweeper.ServerLogic.BusinessLogic;
using MineSweeper.ServerLogic.Exceptions;
using MineSweeper.Service.DataContracts.Faults;
using MineSweeper.Service.ServiceContracts;

namespace MineSweeper.Service
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
	public class AccountService : IAccountService
	{
		public Guid AccountLogin(string username, string password)
		{
			IAccountController accountController = new AccountController();
			Guid res = Guid.Empty;
			try
			{
				res = accountController.VerifyPassword(username, password);
                Console.WriteLine("User: " + username + " logged in");
				return res;
			}
			catch (InvalidUserCredentialsException e)
			{
				throw new FaultException<InvalidUserCredentialsFault>(new InvalidUserCredentialsFault(e.Message));
			}
		}

		public bool EditUserDetails(int id, string oldPassword, string newPassword)
		{
			IAccountController accountController = new AccountController();
			try
			{
				bool res = accountController.EditUserDetails(id, oldPassword, newPassword);
				return res;
			}
			catch (DBException)
			{

				throw new FaultException<DBFault>(new DBFault("Could not edit account information."));
			}
		}

		public Account GetAccount(string username)
		{
			IAccountController accountController = new AccountController();
			try
			{
				Account res = accountController.GetAccount(username);
				return res;
			}
			catch (DBException)
			{

				throw new FaultException<DBFault>(new DBFault("Could not retrive account due to an database error."));
			}
		}

		public Account GetAccountById(int accountId)
		{
			IAccountController accountController = new AccountController();
			try
			{
				return accountController.GetAccount(accountId);
			}
			catch (Exception)
			{

				throw new FaultException<DBFault>(new DBFault("Could not retrive account due to an database error."));
			}
		}

		public int GetLossCount(int accountId)
		{
			IAccountController accountController = new AccountController();
			try
			{
				return accountController.GetLossCount(accountId);

			}
			catch (DBException)
			{
				throw new FaultException<DBFault>(new DBFault("Could not retrive account losses due to an database error."));
			}
		}

		public int GetWinCount(int accountId)
		{
			IAccountController accountController = new AccountController();
			try
			{
				return accountController.GetWinCount(accountId);
			}
			catch (DBException)
			{

				throw new FaultException<DBFault>(new DBFault("Could not retrive account wins due to an database error."));
			}
		}

		public void RegisterNewUser(string username, string password)
		{
			IAccountController accountController = new AccountController();
			try
			{
				accountController.RegisterUser(username, password);
                Console.WriteLine("User: " + username + " registered");
            }
            catch (UsernameException ex)
			{
				throw new FaultException<UsernameFault>(new UsernameFault(ex.Message));
			}
		}
		public Account GetAccountWithKey(string username, Guid sessionKey)
		{
			IAccountController accountController = new AccountController();
			try
			{
				Account res = accountController.GetAccountWithKey(username, sessionKey);
				return res;
			}
			catch (DBException)
			{

				throw new FaultException<DBFault>(new DBFault("Could not retrive account due to an database error."));
			}
		}

		public bool DeactivateAccount(int id, Guid sessionKey)
		{
			IAccountController accountController = new AccountController();
			try
			{
				return accountController.DeactivateAccount(id, sessionKey);
			}
			catch (DBException)
			{

				throw new FaultException<DBFault>(new DBFault("Could not retrive account due to an database error."));
			}
		}

		public bool ValidateSession(int id, Guid sessionKey)
		{
			IAccountController accountController = new AccountController();
			try
			{
				return accountController.ValidateSession(id, sessionKey);
			}
			catch (DBException)
			{
				throw new FaultException<InvalidSessionFault>(new InvalidSessionFault());
			}
		}
	}
}
