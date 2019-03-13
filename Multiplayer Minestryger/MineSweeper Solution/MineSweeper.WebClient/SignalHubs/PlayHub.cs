using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using MineSweeper.Model;
using MineSweeper.ServerLogic.GameLogic;
using MineSweeper.Service.DataContracts;
using MineSweeper.Service.Proxy;
using MineSweeper.Service.ServiceContracts;
using Newtonsoft.Json;

namespace MineSweeper.WebHost.SignalR
{
	[CallbackBehavior(UseSynchronizationContext = true)]
	public class PlayHub : Hub, IGameCallbackService
	{
		private IHubContext context;

		private static Dictionary<int, List<string>> webConnections = new Dictionary<int, List<string>>();
		private static Dictionary<int, GameDuplexProxy> playerProxys = new Dictionary<int, GameDuplexProxy>();
		
		public PlayHub()
		{
			context = GlobalHost.ConnectionManager.GetHubContext<PlayHub>();
		}

		public void TryActivateField(string field, int accountId, string guid)
		{
			string[] temp = field.Split('_');
			int x = int.Parse(temp[0]);
			int y = int.Parse(temp[1]);
			Guid accountGuid = Guid.Parse(guid);
			playerProxys[accountId].MakeMove(x, y, accountGuid, accountId);
		}

		private void ConvertJaggedMinefield(GameData gameData)
		{
			gameData.Minefield = new FieldData[gameData.Width, gameData.Height];
			for (int y = 0; y < gameData.Height; y++)
			{
				for (int x = 0; x < gameData.Width; x++)
				{
					gameData.Minefield[x, y] = gameData.JaggedMinefield[y][x];
				}
			}
		}

		public void EndGame()
		{
			context.Clients.All.CallOnEndGame();
		}

		public void UpdateClient(GameData gameData)
		{
			ConvertJaggedMinefield(gameData);
			List<string> clients = null;
			if (webConnections.TryGetValue(gameData.GameId, out clients))
			{
				foreach (string element in clients)
				{
					context.Clients.Client(element).UpdateGameData(gameData);
				}
			}
		}

		public void JoinGame(int accountId, Guid guid)
		{
			int joinedGame = -1;
			GameDuplexProxy callbackProxy;
			if (playerProxys.TryGetValue(accountId, out callbackProxy))
			{
				joinedGame = callbackProxy.JoinWebGame(accountId, GameType.QuickMatch, guid);
			}
			else
			{
				GameDuplexProxy newProxy = new GameDuplexProxy(new InstanceContext(this), "duplexGameService");
				joinedGame = newProxy.JoinWebGame(accountId, GameType.QuickMatch, guid);
				playerProxys.Add(accountId, newProxy);
			}

			List<string> clientId;
			if (webConnections.TryGetValue(joinedGame, out clientId))
			{
				clientId.Add(Context.ConnectionId);
			}
			else
			{
				List<string> temp = new List<string>();
				temp.Add(Context.ConnectionId);
				webConnections.Add(joinedGame, temp);
			}
		}
		public void ConsoleMessage(string message)
		{
			//Do nothing because web development has stopped
		}
	}

}