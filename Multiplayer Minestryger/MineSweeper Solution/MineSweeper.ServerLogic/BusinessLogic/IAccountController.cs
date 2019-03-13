using System;
using MineSweeper.Model;

namespace MineSweeper.ServerLogic.BusinessLogic
{
    public interface IAccountController
    {
        bool DeactivateAccount(int id, Guid sessionKey);
        bool EditUserDetails(int id, string oldPassword, string newPassword);
        Account GetAccount(int accountId);
        Account GetAccount(string username);
        Account GetAccountWithKey(string username, Guid sessionKey);
        int GetLossCount(int accountId);
        int GetWinCount(int accountId);
        void RegisterUser(string username, string password);
        bool ValidateSession(int id, Guid sessionKey);
        Guid VerifyPassword(string username, string password);
    }
}