using MineSweeper.Model;
using MineSweeper.ServerLogic.DB;
using MineSweeper.ServerLogic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;

namespace MineSweeper.ServerLogic.BusinessLogic
{
    public class AccountController : IAccountController
    {
        private readonly int ITERATIONCOUNT = 32000;
        private static Dictionary<int, Guid> activeSessions = new Dictionary<int, Guid>();
        private static ReaderWriterLock guidLock = new ReaderWriterLock();
        private static int LockTimeout = 50000;
        private AccountDB accountDb;
        //No starting or ending spaces, no other signs that ) ( . - _ a-z 0-9
        private static string patteren = @"^[-a-zA-Z0-9-()-._-]+(\s+[-a-zA-Z0-9-()-._-]+)*$";

        public AccountController()
        {
            accountDb = new AccountDB();
        }

        public void RegisterUser(string username, string password)
        {
            bool Allowed = Regex.IsMatch(username, patteren);
            if (!Allowed || username.Length > 14)
            {
                throw new UsernameException("Username may only be 14 characters and " +
                    "contain: a-z , 0-9 , - , . , _ , ( , ) ");
            }
            byte[] salt = GenerateRandomSalt();

            byte[] hash = GenerateHash(password, salt);

            Account newAccount = new Account();
            newAccount.Username = username;
            newAccount.Salt = Convert.ToBase64String(salt);
            newAccount.Password = Convert.ToBase64String(hash);

            accountDb.InsertAccount(newAccount);
        }

        /// <summary>
        /// KAN VÆRE AT DEN IKKE SKAL BRUGES
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public Account GetAccountWithKey(string username, Guid sessionKey)
        {
            AccountDB accountDB = new AccountDB();
            Account account = accountDB.GetAccount(username);
			//Console.WriteLine(sessionKey);
			//Console.WriteLine(account.SessionKey);
			if (account.SessionKey.Equals(sessionKey))
            {
                return account;
            }
            else
            {
                return null;
            }
        }

        internal Account GetAccount(int id, Guid sessionKey)
        {
            AccountDB accountDB = new AccountDB();
            Account account = accountDB.GetAccount(id);
            if (account.SessionKey.Equals(sessionKey))
            {
                return account;
            }
            else
            {
                return null;
            }
        }
        public Account GetAccount(string username)
        {
            AccountDB accountDB = new AccountDB();
            Account account = accountDB.GetAccount(username);
            return account;
        }

        public Account GetAccount(int accountId)
        {
            AccountDB accountDB = new AccountDB();
            return accountDB.GetAccount(accountId);
        }

        public Guid VerifyPassword(string username, string password)
        {

            Account account = accountDb.GetAccount(username);

            if (account == null)
            {
                throw new InvalidUserCredentialsException();
            }
            else
            {

                //Console.WriteLine("salt: " + account.Salt + " hashed password: " + account.Password);

                byte[] salt = Convert.FromBase64String(account.Salt);

                byte[] hash = GenerateHash(password, salt);

                byte[] dbHash = Convert.FromBase64String(account.Password);


                if (CompareHashes(hash, dbHash))
                {
                    Guid guid = Guid.NewGuid();
					accountDb.SetSession(account.Id, guid);
					guidLock.AcquireWriterLock(LockTimeout);
                    if (activeSessions.ContainsKey(account.Id))
                    {
                        activeSessions[account.Id] = guid;
                    }
                    else
                    {
                        activeSessions.Add(account.Id, guid);
                    }
                    guidLock.ReleaseWriterLock();
                    //Console.WriteLine(username + " er logget ind!");
                    return guid;
                }
                else
                {
                    throw new InvalidUserCredentialsException();
                }
            }

        }

        public int GetWinCount(int accountId)
        {
            return accountDb.GetWinCount(accountId);
        }

        public int GetLossCount(int accountId)
        {
            return accountDb.GetLossCount(accountId);
        }

        public bool ValidateSession(int id, Guid sessionKey)
        {
            Guid guid;
            guidLock.AcquireReaderLock(LockTimeout);
            bool valid = false;
            if (activeSessions.TryGetValue(id, out guid))
            {
                if (guid.Equals(sessionKey))
                {
                    valid = true;
                }
            }
            guidLock.ReleaseReaderLock();
            return valid;
        }


        public bool EditUserDetails(int id, string oldPassword, string newPassword)
        {

            Account account = accountDb.GetAccount(id);

            byte[] dbSalt = Convert.FromBase64String(account.Salt);
            byte[] dbHash = Convert.FromBase64String(account.Password);
            byte[] oldHash = GenerateHash(oldPassword, dbSalt);

            if (CompareHashes(oldHash, dbHash))
            {
                byte[] newHash = GenerateHash(newPassword, dbSalt);
                string hashedNewPassword = Convert.ToBase64String(newHash);
                accountDb.UpdateAccount(id, hashedNewPassword);

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeactivateAccount(int id, Guid sessionKey)
        {
            bool res = false;

            Account account = accountDb.GetAccount(id);

            if (accountDb.DeactivateAccount(id))
            {
                return res = true;
            }
            return res;
        }

        private bool CompareHashes(byte[] hash, byte[] dbHash)
        {
            return hash.SequenceEqual(dbHash);
        }

        // Hash Generation for registration and verification
        private byte[] GenerateHash(string password, byte[] saltVal)
        {
            byte[] hash;

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltVal, ITERATIONCOUNT))
            {
                hash = pbkdf2.GetBytes(20);
            }

            return hash;
        }

        private byte[] GenerateRandomSalt()
        {
            byte[] salt = new byte[16];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }
    }
}
