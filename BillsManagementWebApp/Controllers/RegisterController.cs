using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillsManagementWebApp.ViewModels;

namespace BillsManagementWebApp.Controllers
{
    public class RegisterController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserViewModel objUserViewModel)
        {
            return null;
        }
    }
}