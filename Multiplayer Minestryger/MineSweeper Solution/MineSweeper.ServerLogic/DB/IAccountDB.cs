using MineSweeper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.ServerLogic.DB
{
	interface IAccountDB
	{
		void InsertAccount(Account user);
		Account GetAccount(string username);
		Account GetAccount(int id);
		bool DeleteAccount(int id);
        int GetWinCount(int accountId);
        int GetLossCount(int accountId);
    }
}
