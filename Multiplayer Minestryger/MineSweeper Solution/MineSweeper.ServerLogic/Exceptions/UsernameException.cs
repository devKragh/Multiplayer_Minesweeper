using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.ServerLogic.Exceptions
{
	public class UsernameException : Exception
	{
		public UsernameException(string message) : base(message) { }
	}
}
