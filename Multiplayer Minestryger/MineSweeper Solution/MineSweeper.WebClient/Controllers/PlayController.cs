using MineSweeper.Service.DataContracts;
using MineSweeper.WebHost.BusinessLogic;
using MineSweeper.WebHost.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace MineSweeper.WebHost.Controllers
{
	public class PlayController : Controller
	{
		// GET: Game
		public ActionResult Index()
		{
			if (!ValidationController.ValidateAccountSession(Request))
				return RedirectToAction("Login", "Account");

			return View();
		}

		[HttpGet]
		public ActionResult Game()
		{
			if (!ValidationController.ValidateAccountSession(Request))
				return RedirectToAction("Login", "Account");

			return View("Game");
		}
	}
}