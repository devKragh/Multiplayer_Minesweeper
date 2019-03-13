using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Service.DataContracts.Faults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.Proxy
{
    [ServiceContract(CallbackContract = typeof(ServiceContracts.IGameCallbackService), SessionMode = SessionMode.Required)]
    public interface IGameService
    {
        [OperationContract]
        [FaultContract(typeof(NotImplementedFault))]
        void JoinActiveGame(int accountId, Guid guid);

        [OperationContract]
        int JoinWebGame(int accountId, GameType gameType, Guid guid);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        [FaultContract(typeof(InvalidIFieldActivationFault))]
        [FaultContract(typeof(InvalidSessionFault))]
        [FaultContract(typeof(DBFault))]
        [FaultContract(typeof(UnclassifiedFault))]
        void MakeMove(int x, int y, Guid guid, int accountId);

        [OperationContract(IsInitiating = true, IsTerminating = false)]
        [FaultContract(typeof(DBFault))]
        [FaultContract(typeof(InvalidSessionFault))]
        [FaultContract(typeof(UnclassifiedFault))]
        void JoinNewGame(int accountId, GameType gameType, Guid guid);

        [OperationContract(IsInitiating = false, IsTerminating = false)]
        [FaultContract(typeof(DBFault))]
        [FaultContract(typeof(UnclassifiedFault))]
        void Ready();

        [OperationContract(IsTerminating = true, IsInitiating = false)]
        void Disconnect();

        [OperationContract]
        Task JoinNewGameAsync(int accountId, GameType gameType, Guid guid);

        [OperationContract]
        Task MakeMoveAsync(int x, int y, Guid guid, int accountId);

        [OperationContract]
        Task ReadyAsync();

        [OperationContract]
        Task DisconnectAsync();
    }
}
