using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.ServerLogic.Exceptions
{
	public class InternalServerException : Exception
	{
		public InternalServerException() : base("Internal Server Exception. Try again later") { }
	}
}
