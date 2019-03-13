using MineSweeper.WebHost.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MineSweeper.WebHost
{
	public partial class DynamicData : System.Web.UI.Page
	{
		DataClass dataClass;
		protected void Page_Load(object sender, EventArgs e)
		{
			dataClass = DataClass.GetInstance();
			form1.InnerHtml = "Fuck dig";
			Thread thread = new Thread(() =>
		   {
			   while (true)
			   {
					AddDataToWeb();
				   Thread.Sleep(500);
			   }
		   });
			thread.Start();
		}


		private void AddDataToWeb()
		{
			List<string> data = dataClass.GetData();
			foreach (string element in data)
			{
				form1.InnerHtml += element;
			}
		}
	}
}