using MineSweeper.Model;
using MineSweeper.Service.ServiceContracts;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace MineSweeper.Service.Proxy
{
    public class AccountProxy : ClientBase<IAccountService>, IAccountService
    {
        public AccountProxy(string configName) : base(configName) { }
        public AccountProxy(Binding binding, EndpointAddress endpointAddress) : base(binding, endpointAddress)
        { }
        public void RegisterNewUser(string username, string password)
        {
            Channel.RegisterNewUser(username, password);
        }

        public bool ValidateSession(int id, Guid sessionKey)
        {
            return Channel.ValidateSession(id, sessionKey);
        }

        public Guid AccountLogin(string username, string password)
        {
            return Channel.AccountLogin(username, password);
        }

        public Account GetAccount(string username)
        {
            return Channel.GetAccount(username);
        }

        public int GetLossCount(int accountId)
        {
            return Channel.GetLossCount(accountId);
        }

        public int GetWinCount(int accountId)
        {
            return Channel.GetWinCount(accountId);
        }

        public Account GetAccountWithKey(string username, Guid sessionKey)
        {
            return Channel.GetAccountWithKey(username, sessionKey);
        }

        public bool EditUserDetails(int id, string oldPassword, string newPassword)
        {
            return Channel.EditUserDetails(id, oldPassword, newPassword);
        }

        public bool DeactivateAccount(int id, Guid sessionKey)
        {
            return Channel.DeactivateAccount(id, sessionKey);
        }

        public Account GetAccountById(int accountId)
        {
            return Channel.GetAccountById(accountId);
        }
    }
}
