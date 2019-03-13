using MineSweeper.Model;
using MineSweeper.Service.DataContracts.Faults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MineSweeper.Service.ServiceContracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRegisterService" in both code and config file together.
    [ServiceContract]
    public interface IAccountService
    {

        [OperationContract]
		[FaultContract(typeof(UsernameFault))]
        void RegisterNewUser(string username, string password);

        [OperationContract]
        [FaultContract(typeof(InvalidUserCredentialsFault))]
        Guid AccountLogin(string username, string password);

        [OperationContract]
        Account GetAccount(string username);

        [OperationContract]
        Account GetAccountById(int accountId);

        [OperationContract]
        bool ValidateSession(int id, Guid sessionKey);

        [OperationContract]
        int GetLossCount(int accountId);

        [OperationContract]
        int GetWinCount(int accountId);
    
        [OperationContract]
        Account GetAccountWithKey(string username, Guid sessionKey);

        [OperationContract]
        bool EditUserDetails(int id, string oldPassword, string newPassword);

        [OperationContract]
        bool DeactivateAccount(int id, Guid sessionKey);
    }
}
