using MineSweeper.ServerLogic.GameLogic;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace MineSweeper.Service.Proxy
{
    public class GameDuplexProxy : DuplexClientBase<IGameService>, IGameService
    {
        public GameDuplexProxy(InstanceContext instanceContext, string endpointName) : base(instanceContext, endpointName) { }

        public void Disconnect()
        {
            Channel.Disconnect();
        }

        public Task DisconnectAsync()
        {
            return Channel.DisconnectAsync();
        }

        public void JoinActiveGame(int accountId, Guid guid)
        {
            Channel.JoinActiveGame(accountId, guid);
        }

        public void JoinNewGame(int accountId, GameType gameType, Guid guid)
        {
            Channel.JoinNewGame(accountId, gameType, guid);
        }

        public Task JoinNewGameAsync(int accountId, GameType gameType, Guid guid)
        {
            return Channel.JoinNewGameAsync(accountId, gameType, guid);
        }

        public int JoinWebGame(int accountId, GameType gameType, Guid guid)
		{
			return Channel.JoinWebGame(accountId, gameType, guid);
		}

		public void MakeMove(int x, int y, Guid guid, int accountId)
        {
            Channel.MakeMove(x, y, guid, accountId);
        }

        public Task MakeMoveAsync(int x, int y, Guid guid, int accountId)
        {
            return Channel.MakeMoveAsync(x, y, guid, accountId);
        }

        public void Ready()
        {
            Channel.Ready();
        }

        public Task ReadyAsync()
        {
            return Channel.ReadyAsync();
        }
    }
}
