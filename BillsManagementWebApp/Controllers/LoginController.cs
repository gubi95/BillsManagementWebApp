using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BillsManagementWebApp.ViewModels;
using BillsManagementWebApp.Models;
using BillsManagementWebApp.Shared;

namespace BillsManagementWebApp.Controllers
{
    public class LoginController : Controller
    {   
        [HttpGet]        
        public ActionResult Index()
        {
            if (SessionManager.GetCurrentUser() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(LoginViewModel objLoginViewModel)
        {
            string strUsername = "" + objLoginViewModel.Username;
            string strPassword = "" + objLoginViewModel.Password;

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Models.User objUser = objApplicationDBContext
                                    .Users
                                    .Include("Bills")
                                    .Include("Bills.Entries")
                                    .Include("Bills.Entries.Category")
                                    .Where(x => x.Username.Equals(strUsername))
                                    .FirstOrDefault();

            if (objUser != null)
            {
                string strHashedPassword = objUser.Password;
                bool bIsPasswordCorrect = PasswordHasher.CompareHashes(strPassword, strHashedPassword);

                if (bIsPasswordCorrect)
                {
                    SessionManager.SetCurrentUser(objUser);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    SessionManager.SetCurrentUser(null);
                    TempData["error"] = "wrong_auth";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["error"] = "wrong_auth";
                return RedirectToAction("Index");
            }
        }
    }
}