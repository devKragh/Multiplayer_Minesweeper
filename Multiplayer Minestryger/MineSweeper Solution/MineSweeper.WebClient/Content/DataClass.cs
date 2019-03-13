using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace MineSweeper.WebHost.Content
{
	public class DataClass
	{
		private static DataClass instance;
		private List<string> dynamiskData;
		private DataClass()
		{
			dynamiskData = new List<string>();
			Thread thread = new Thread(() =>
			{
				lock (dynamiskData)
				{
					dynamiskData.Add("Random String");
				}
				Thread.Sleep(1000);
			});
			thread.Start();
		}

		public static DataClass GetInstance()
		{
			if (instance == null)
			{
				instance = new DataClass();
			}
			return instance;
		}
		
		public List<string> GetData()
		{
			lock (dynamiskData)
			{
				return dynamiskData;
			}
		}
	}
}