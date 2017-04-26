using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillsManagementWebApp.ViewModels;
using BillsManagementWebApp.Shared;

namespace BillsManagementWebApp.Controllers
{
    public class HomeController : Controller
    {
        [OnlyLoggedInUser]
        [HttpGet]
        public ActionResult Index()
        {
            UserViewModel objUserViewModel = new UserViewModel();
            objUserViewModel.ApplyFromModel(SessionManager.GetCurrentUser());
            return View(objUserViewModel);
        }

        [HttpPost]
        public ActionResult SignOut()
        {
            SessionManager.SetCurrentUser(null);
            return RedirectToAction("Index", "Login");
        }
    }
}