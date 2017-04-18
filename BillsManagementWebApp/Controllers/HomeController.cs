using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillsManagementWebApp.Shared;

namespace BillsManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        [LoggedInUser]
        public ActionResult Index()
        {
            if (SessionManager.GetCurrentUser() == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }
    }
}