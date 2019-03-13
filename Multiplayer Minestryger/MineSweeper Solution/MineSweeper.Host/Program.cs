using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper.Host
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("MINESWEEPER HOST");

			using (ServiceHost servicehost = new ServiceHost(typeof(Service.AccountService)))
			{
				servicehost.Open();

				DisplayHostInfo(servicehost);

				Console.ReadLine();
			}
		}

		static void DisplayHostInfo(ServiceHost host)
		{
			Console.WriteLine();
			Console.WriteLine("-- Host Info --");

			foreach (System.ServiceModel.Description.ServiceEndpoint se in host.Description.Endpoints)
			{
				Console.WriteLine($"Address: {se.Address}");
				Console.WriteLine($"Binding: {se.Binding.Name}");
				Console.WriteLine($"Contract: {se.Contract.Name}");
			}
			Console.WriteLine("---------------");

		}
	}
}
