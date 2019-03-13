using MineSweeper.Model;
using MineSweeper.ServerLogic.Exceptions;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace MineSweeper.ServerLogic.DB
{
    public class AccountDB : IAccountDB
    {
        private string connectionString;
        private IGameDB gameDb;
        public AccountDB()
        {
            connectionString = ConfigurationManager.ConnectionStrings["localDB"].ConnectionString;
            gameDb = new GameDB();
        }

        public void InsertAccount(Account account)
        {
            if (GetAccount(account.Username) == null)
            {
                
                using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        using (SqlCommand insertAccountCmd = new SqlCommand("INSERT INTO Account VALUES (@salt, @username, @password, null, @rankpoints, 1)"))
                        {
                            connection.Open();
                            insertAccountCmd.Connection = connection;
                            insertAccountCmd.Parameters.AddWithValue("salt", account.Salt);
                            insertAccountCmd.Parameters.AddWithValue("username", account.Username);
                            insertAccountCmd.Parameters.AddWithValue("password", account.Password);
                            insertAccountCmd.Parameters.AddWithValue("rankpoints", 0);
                            insertAccountCmd.ExecuteNonQuery();
                        }
                    }
                    transactionScope.Complete();
                }
            }
            else
            {
                throw new UsernameException("Username already exist");
            }
        }

        public int GetWinCount(int accountId)
        {
            return gameDb.GetWinsByUserId(accountId);
        }

        public int GetLossCount(int accountId)
        {
            return gameDb.GetLossesByUserId(accountId);
        }

        public void SetSession(int accountId, Guid sessionKey)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew, transactionOptions))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    int rowsAffected = -1;
                    connection.Open();
                    using (SqlCommand setSessionCmd = new SqlCommand("UPDATE Account SET sessionkey=@sessionKey WHERE id=@id", connection))
                    {
                        setSessionCmd.Parameters.AddWithValue("sessionKey", sessionKey.ToString());
                        setSessionCmd.Parameters.AddWithValue("id", accountId);
                        rowsAffected = setSessionCmd.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            throw new DBException("Tried to set session and failed.");    
                        }

                    }
                }
                transactionScope.Complete();
            }
        }

        public Account GetAccount(string username)
        {
            Account res = null;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand selectAccountCmd = new SqlCommand("SELECT * FROM Account WHERE username = @username"))
                    {
                        connection.Open();
                        selectAccountCmd.Connection = connection;
                        selectAccountCmd.Parameters.AddWithValue("username", username);
                        SqlDataReader accountReader = selectAccountCmd.ExecuteReader();
                        res = SetAccountInformation(accountReader);
                    }
                    transactionScope.Complete();
                }
            }
            return res;
        }

        private Account SetAccountInformation(SqlDataReader accountReader)
        {
            Account res = null;
            if (accountReader.Read())
            {
                res = new Account();
                res.Id = accountReader.GetInt32(0);
                res.Salt = accountReader.GetString(1);
                res.Username = accountReader.GetString(2);
                res.Password = accountReader.GetString(3);
                if (!accountReader.IsDBNull(4))
                {
                    res.SessionKey = Guid.Parse(accountReader.GetString(4));
                }
                res.Rankpoints = accountReader.GetInt32(5);
                res.Active = accountReader.GetBoolean(6);
                accountReader.Close();
            }
            accountReader.Close();
            return res;
        }

        public Account GetAccount(int id)
        {
            Account res = null;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand selectAccountByIdCmd = new SqlCommand("SELECT * FROM Account WHERE id = @id"))
                    {
                        connection.Open();
                        selectAccountByIdCmd.Connection = connection;
                        selectAccountByIdCmd.Parameters.AddWithValue("id", id);
                        SqlDataReader accountReader = selectAccountByIdCmd.ExecuteReader();
                        res = SetAccountInformation(accountReader);
                    }
                    transactionScope.Complete();
                }
            }
            return res;
        }

        public bool DeleteAccount(int id)
        {
            bool res = false;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand deleteAccountCmd = new SqlCommand("DELETE FROM Account WHERE id = @id"))
                    {

                        connection.Open();
                        deleteAccountCmd.Connection = connection;
                        deleteAccountCmd.Parameters.AddWithValue("id", id);
                        int rowsAffected = deleteAccountCmd.ExecuteNonQuery();
                        if (rowsAffected == 1)
                        {
                            res = true;
                        }
                        else
                        {
                            throw new DBException("Could not delete account. Maybe already deletet or a database error.");
                        }
                    }
                }
                transactionScope.Complete();
            }
            return res;
        }

        public bool UpdateAccount(int id, string password)
        {
            bool res = false;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand updateAccountCmd = new SqlCommand("UPDATE Account SET password=@password WHERE id=@id"))
                    {

                        connection.Open();
                        updateAccountCmd.Connection = connection;

                        updateAccountCmd.Parameters.AddWithValue("id", id);
                        updateAccountCmd.Parameters.AddWithValue("password", password);

                        int rowsAffected = updateAccountCmd.ExecuteNonQuery();
                        if (rowsAffected == 1)
                        {
                            res = true;
                        }
                        else
                        {
                            throw new DBException("Could not update account due to a database error.");
                        }

                    }
                }
                transactionScope.Complete();
            }
            return res;
        }

        public bool DeactivateAccount(int id)
        {
            bool res = false;
            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand deactivateAccountCmd = new SqlCommand("UPDATE Account SET username = concat('deleteduser', @userid), active = 0 WHERE id = @id"))
                    {
                        connection.Open();
                        deactivateAccountCmd.Connection = connection;

                        deactivateAccountCmd.Parameters.AddWithValue("userid", id);
                        deactivateAccountCmd.Parameters.AddWithValue("id", id);

                        int rowsAffected = deactivateAccountCmd.ExecuteNonQuery();
                        if (rowsAffected == 1)
                        {
                            res = true;
                        }
                        else
                        {
                            throw new DBException("Could not delete account. Either the account is already deleted or there was a database error.");
                        }
                    }
                    transactionScope.Complete();
                }
                return res;
            }
        }
    }
}

