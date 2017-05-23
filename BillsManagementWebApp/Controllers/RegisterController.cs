using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillsManagementWebApp.Shared;
using BillsManagementWebApp.ViewModels;
using BillsManagementWebApp.Models;

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
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Models.User objUser = new Models.User();                                  
            objUserViewModel.ApplyToModel(ref objUser);
            objUser.Password = PasswordHasher.GenerateHashForUser(objUserViewModel.Password);
            objApplicationDBContext.Users.Add(objUser);
            objApplicationDBContext.SaveChanges();

            objApplicationDBContext.ProductCategories.AddRange(Constants.GetPreDefinedCategories(objUser));
            objApplicationDBContext.SaveChanges();

            objUser.Bills = new List<Bill>();

            SessionManager.SetCurrentUser(objUser);
                                         
            return RedirectToAction("Index", "Home");
        }
    }
}