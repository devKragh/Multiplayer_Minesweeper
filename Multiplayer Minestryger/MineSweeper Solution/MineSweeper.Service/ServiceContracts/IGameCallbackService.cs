using MineSweeper.Service.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Service.ServiceContracts
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IGameCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void UpdateClient(GameData gameData);

        [OperationContract(IsOneWay = true)]
        void EndGame();

        [OperationContract(IsOneWay = true)]
        void ConsoleMessage(string message);
    }
}
