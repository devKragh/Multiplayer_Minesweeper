using MineSweeper.Model;
using MineSweeper.Service.DataContracts.Faults;
using MineSweeper.Service.Proxy;
using MineSweeper.Service.ServiceContracts;
using System;
using System.ServiceModel;

namespace MineSweeper.DesktopClient.Controller
{
    public class ClientAccountController
    {
        public Guid Guid;
        public Account Account;

        public void Login(string username, string password)
        {
            AccountProxy accountProxy = new AccountProxy("accountService");
            Guid = accountProxy.AccountLogin(username, password);
            Account = accountProxy.GetAccount(username);
        }

        internal int LossCount()
        {
            AccountProxy accountProxy = new AccountProxy("accountService");
            return accountProxy.GetLossCount(Account.Id);
        }

        internal int WinCount()
        {
            AccountProxy accountProxy = new AccountProxy("accountService");
            return accountProxy.GetWinCount(Account.Id);

        }

        public void RegisterUser(string username, string password)
        {
            AccountProxy accountProxy = new AccountProxy("accountService");
            accountProxy.RegisterNewUser(username, password);
        }
    }
}
