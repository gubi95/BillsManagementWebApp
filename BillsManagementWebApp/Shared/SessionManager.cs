using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.Shared
{
    public class SessionManager
    {
        public static void SetCurrentUser(Models.User objUser)
        {
            if (HttpContext.Current.Session[Constants.SessionKey_CurrentUser] == null)
            {
                HttpContext.Current.Session.Add(Constants.SessionKey_CurrentUser, objUser);
            }
            else
            {
                HttpContext.Current.Session[Constants.SessionKey_CurrentUser] = objUser;
            }                                                                           
        }

        public static Models.User GetCurrentUser()
        {
            return HttpContext.Current.Session[Constants.SessionKey_CurrentUser] as Models.User;
        }
    }
}