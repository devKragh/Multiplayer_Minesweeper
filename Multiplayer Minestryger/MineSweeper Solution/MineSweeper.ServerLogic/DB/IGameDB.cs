using System.Collections.Generic;
using MineSweeper.ServerLogic.GameLogic;

namespace MineSweeper.ServerLogic.DB
{
    public interface IGameDB
    {
        bool CheckAllPlayerReadiness(); 
        void DeleteGame(int id); 
        void EndGame(Game game);
        int GetLossesByUserId(int accountId); 
        int GetWinsByUserId(int accountId); 
        void JoinActiveGame(Game game); 
        void JoinGame(Game game); 
        void MakeMove(Game game, List<Field> fieldsToActivate); 
        void SetPlayerReady(Game game); 
        void StartGame(Game game); 
        void UpdateGameObject(Game game); 
        void SetPlayerDead(Game game);
        void UnjoinGame(Game game);
    }
}