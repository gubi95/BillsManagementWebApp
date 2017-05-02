using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.ViewModels;
using BillsManagementWebApp.Models;
using BillsManagementWebApp.Shared;
using Newtonsoft.Json;
using System.IO;

namespace BillsManagementWebApp.Handlers
{
    public class BillHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext objHttpContext)
        {
            string strJSON = new StreamReader(objHttpContext.Request.InputStream).ReadToEnd();
            string strAction = objHttpContext.Request.QueryString["Action"];

            if (strAction.Equals("Create"))
            {
                BillViewModel objBillViewModel = JsonConvert.DeserializeObject<BillViewModel>(strJSON);
                Bill objBill = new Bill();                 
                objBillViewModel.ApplyToModel(ref objBill);                 

                ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

                Models.User objUser = objApplicationDBContext
                                        .Users
                                        .Include("Bills")
                                        .Include("Bills.Shop")
                                        .Include("Bills.BillEntry")
                                        .Include("Bills.BillEntry.Category")
                                        .Where(x => x.UserID == SessionManager.GetCurrentUser().UserID)
                                        .FirstOrDefault();

                objUser.Bills.Add(objBill);
                objApplicationDBContext.SaveChanges();
            }

            objHttpContext.Response.ContentType = "text/plain";
            objHttpContext.Response.Write("Hello World");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}