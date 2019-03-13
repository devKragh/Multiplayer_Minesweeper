using System;

namespace MineSweeper.ServerLogic.Exceptions
{
    public class InvalidFieldActivationException : Exception
    {
        public InvalidFieldActivationException(string message) : base(message)
        {

        }
    }
}
