using MineSweeper.Service.Proxy;
using MineSweeper.WebHost.Controllers;
using System;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MineSweeper.WebHost.BusinessLogic
{
	public class ValidationController
	{
		public static bool ValidateAccountSession(HttpRequestBase requestBase)
		{
			bool res = false;
			HttpCookie cookie = requestBase.Cookies.Get("Session");
			if (cookie != null)
			{
				AccountProxy proxy = new AccountProxy("accountService");
				Guid sessionKey = Guid.Parse(cookie["key"]);
				res = proxy.ValidateSession(int.Parse(cookie["id"]), sessionKey);
			}
			return res;
		}
	}
}