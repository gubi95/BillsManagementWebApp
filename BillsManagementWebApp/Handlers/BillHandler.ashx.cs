using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.ViewModels;
using BillsManagementWebApp.Models;
using BillsManagementWebApp.Shared;
using Newtonsoft.Json;
using System.IO;
using System.Web.SessionState;

namespace BillsManagementWebApp.Handlers
{
    public class BillHandler : IHttpHandler, IRequiresSessionState
    {
        public class ViewStore
        {
            public int StoreID { get; set; }
            public string StoreName { get; set; }
        }

        public class ViewProduct
        {
            public string ProductName { get; set; }
            public double Quantity { get; set; }
            public decimal Price { get; set; }
            public int ProductCategoryID { get; set; }
        }

        private class ViewBill
        {
            public DateTime PurchaseDate { get; set; }
            public ViewStore Store { get; set; }
            public List<ViewProduct> Products { get; set; }            
        }

        private class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public void ProcessRequest(HttpContext objHttpContext)
        {
            string strJSON = new StreamReader(objHttpContext.Request.InputStream).ReadToEnd();
            string strAction = objHttpContext.Request.QueryString["Action"];

            if (strAction.Equals("Create"))
            {
                try
                {
                    ViewBill objViewBill = JsonConvert.DeserializeObject<ViewBill>(strJSON);

                    ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

                    int nCurrentUserID = SessionManager.GetCurrentUser().UserID;

                    Models.User objUser = objApplicationDBContext
                                            .Users
                                            .Include("Bills")
                                            .Where(x => x.UserID == nCurrentUserID).FirstOrDefault();

                    List<ProductCategory> listProductCategory = objApplicationDBContext
                                                                .ProductCategories
                                                                .ToList();

                    Shop objShop = objApplicationDBContext
                                            .Shops
                                            .Include("UserOwner")
                                            .Where(x => x.UserOwner.UserID == nCurrentUserID &&
                                                x.ShopID == objViewBill.Store.StoreID)
                                            .FirstOrDefault();

                    if (objShop == null)
                    {
                        objShop = new Shop()
                        {
                            ShopName = objViewBill.Store.StoreName,
                            UserOwner = objApplicationDBContext.Users.Where(x => x.UserID == nCurrentUserID).FirstOrDefault()
                        };

                        objApplicationDBContext.Shops.Add(objShop);
                        objApplicationDBContext.SaveChanges();
                    }

                    List<BillEntry> listBillEntry = new List<BillEntry>();

                    foreach (ViewProduct objViewProduct in objViewBill.Products)
                    {
                        listBillEntry.Add(new BillEntry()
                        {
                            Category = listProductCategory.Find(x => x.ProductCategoryID == objViewProduct.ProductCategoryID),
                            Price = objViewProduct.Price,
                            ProductName = objViewProduct.ProductName,
                            Quantity = objViewProduct.Quantity
                        });
                    }

                    Bill objBill = new Bill()
                    {
                        PurchaseDate = objViewBill.PurchaseDate,
                        Shop = objShop,
                        Entries = listBillEntry
                    };

                    objUser.Bills.Add(objBill);
                    objApplicationDBContext.SaveChanges();

                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(new Response() { Success = true, Message = "OK" }));
                }
                catch (Exception ex)
                {
                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(new Response() { Success = false, Message = ex.ToString() }));
                }
            }
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