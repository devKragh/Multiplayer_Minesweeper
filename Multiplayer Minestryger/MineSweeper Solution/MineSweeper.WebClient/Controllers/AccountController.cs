using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using MineSweeper.Model;
using MineSweeper.Service;
using MineSweeper.Service.Proxy;
using MineSweeper.WebHost.BusinessLogic;
using MineSweeper.WebHost.Models;

namespace MineSweeper.WebHost.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel model)
        {
			try
			{
				var username = model.Username;
				var password = model.Password;
				if (password.Equals(model.ConfirmPassword))
				{
					AccountProxy proxy = new AccountProxy("accountService");
					proxy.RegisterNewUser(username, password);
					proxy.Close();
					return RedirectToAction("Login");
				}
				else
				{
					return View(new RegisterViewModel());
				}
			}
			catch (FaultException)
			{
				RegisterViewModel registerViewModel = new RegisterViewModel();
				registerViewModel.AlreadyExist = true;
				return View(registerViewModel);
			}
			catch (Exception)
			{
				return RedirectToAction("Contact", "Home");
			}
            
        }

        public ActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
			try
			{
				var username = model.Username;
				var password = model.Password;
				AccountProxy proxy = new AccountProxy("accountService");
				Guid sessionKey = proxy.AccountLogin(username, password);
				if (!sessionKey.Equals(Guid.Empty))
				{
					Account account = proxy.GetAccount(username);
                    if (account.Active)
                    {
                        CreateSessionCookie(account, sessionKey);
                        return RedirectToAction("Index", "Play");
                    }
                }
			}
			catch (FaultException)
			{
				LoginViewModel loginViewModel = new LoginViewModel();
				loginViewModel.WrongLoginInfo = true;
				return View(loginViewModel);
			}
			catch (Exception)
			{
				//Should be handled
			}
            
            return View(new LoginViewModel());
        }

        public ActionResult LogOff()
        {
            if (Request.Cookies["Session"] != null)
            {
                HttpCookie cookie = new HttpCookie("Session");
                cookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(cookie);
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Details");
        }

        public ActionResult Details()
        {
            if (!ValidationController.ValidateAccountSession(Request))
                return RedirectToAction("Login", "Account");

            HttpCookie cookie = Request.Cookies.Get("Session");
            if (cookie != null)
            {
                Guid sessionKey = Guid.Parse(cookie["key"]);
                AccountProxy proxy = new AccountProxy("accountService");
                Account account = proxy.GetAccountWithKey(cookie["name"], sessionKey);
                if (account != null)
                {
                    return View(account);
                }
            }
            return RedirectToAction("Login");
        }

        public ActionResult Edit()
        {
            if (!ValidationController.ValidateAccountSession(Request))
                return RedirectToAction("Login", "Account");

            return View(new AccountEditDetailsViewModel());
        }

        [HttpPost]
        public ActionResult Edit(AccountEditDetailsViewModel model)
        {
            if (!ValidationController.ValidateAccountSession(Request))
                return RedirectToAction("Login", "Account");

            string inputOldPassword = model.OldPassword;
            string inputNewPassword = model.NewPassword;

            HttpCookie cookie = Request.Cookies.Get("Session");
            if (cookie != null)
            {
                Guid sessionKey = Guid.Parse(cookie["key"]);
                AccountProxy proxy = new AccountProxy("accountService");
                Account account = proxy.GetAccountWithKey(cookie["name"], sessionKey);
                if (account != null)
                {
                    proxy.EditUserDetails(account.Id, inputOldPassword, inputNewPassword);
                    return View("Index");
                }
            }
            return View(new AccountEditDetailsViewModel());
        }

        [HttpPost]
        public ActionResult _PartialDelete(AccountDetailsViewModel m)
        {
            if (!ValidationController.ValidateAccountSession(Request))
                return RedirectToAction("Login", "Account");

            HttpCookie cookie = Request.Cookies.Get("Session");
            if (cookie != null)
            {
                Guid sessionKey = Guid.Parse(cookie["key"]);
                AccountProxy proxy = new AccountProxy("accountService");
                Account account = proxy.GetAccountWithKey(cookie["name"], sessionKey);
                if (account != null)
                {
                    proxy.DeactivateAccount(account.Id, account.SessionKey);
                    return LogOff();
                }
            }
            return View("Index", "Account");
        }

        public ActionResult _PartialDelete()
        {
            if (!ValidationController.ValidateAccountSession(Request))
                return RedirectToAction("Login", "Account");

            return PartialView("_PartialDelete");
        }

        private void CreateSessionCookie(Account account, Guid sessionKey)
        {
            HttpCookie cookie = new HttpCookie("Session");

            cookie["key"] = sessionKey.ToString();
            cookie["name"] = account.Username;
            cookie["rankPoints"] = Convert.ToString(account.Rankpoints);
            cookie["id"] = Convert.ToString(account.Id);
            Response.Cookies.Add(cookie);
            Console.WriteLine("Cookie created and added");
        }
    }
}