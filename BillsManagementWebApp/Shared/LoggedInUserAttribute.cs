using System;
using BillsManagementWebApp.Shared;
using System.Web.Mvc;

namespace BillsManagementWebApp.Shared
{
    public class OnlyLoggedInUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext objActionExecutingContext)
        {
            if (SessionManager.GetCurrentUser() == null)
            {
                objActionExecutingContext.Result = new RedirectResult("/Login");
            }            
        }
    }
}