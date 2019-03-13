using MineSweeper.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost accountServiceHost = new ServiceHost(typeof(AccountService));
            ServiceHost gameServiceHost = new ServiceHost(typeof(GameService));
            accountServiceHost.Open();
            Console.WriteLine("AccountService started");
            gameServiceHost.Open();
            Console.WriteLine("GameService started");
            Console.ReadLine();
            gameServiceHost.Close();
            accountServiceHost.Close();
        }
    }
}
