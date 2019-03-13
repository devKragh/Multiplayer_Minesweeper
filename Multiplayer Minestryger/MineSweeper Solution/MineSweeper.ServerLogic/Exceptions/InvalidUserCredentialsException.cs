using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.ServerLogic.Exceptions
{
    public class InvalidUserCredentialsException : Exception
    {
        public InvalidUserCredentialsException() : base("Incorrect username or password")
        {
        }
    }
}
