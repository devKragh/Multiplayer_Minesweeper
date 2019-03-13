using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MineSweeper.WebHost.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Multiplayer Minesweeper.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Send your problems to 1067353@ucn.dk.";

            return View();
        }
    }
}