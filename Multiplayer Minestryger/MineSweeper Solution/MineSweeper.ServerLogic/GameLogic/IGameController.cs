namespace MineSweeper.ServerLogic.GameLogic
{
    public interface IGameController : IUpdateable
    {
        Game Game { get; set; }
        bool AllPlayersReady { get; set; }
        void TryActivateField(int x, int y);
        void JoinActiveGame();
        void JoinNewGame(GameType gameType);
        void Ready();
        bool CheckAllPlayerReadiness();
        void EndGame();
        void UnjoinGame();
    }
}