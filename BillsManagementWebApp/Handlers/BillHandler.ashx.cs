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
            public int ProductID { get; set; }
            public string ProductName { get; set; }
            public double Quantity { get; set; }
            public decimal Price { get; set; }
            public int ProductCategoryID { get; set; }
        }

        public class ViewBill
        {
            public int BillID { get; set; }
            public DateTime PurchaseDate { get; set; }
            public ViewStore Store { get; set; }
            public List<ViewProduct> Products { get; set; }            
        }

        private class Response
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public Bill Bill { get; set; }
            public string NewPriceFormatted { get; set; }
            public int NewProductsCount { get; set; }
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
                                                                .Where(x => x.UserOwnerId == nCurrentUserID)
                                                                .ToList();

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
                        Entries = listBillEntry
                    };

                    AssignShopToBill(objApplicationDBContext, ref objBill, objViewBill, nCurrentUserID);

                    objUser.Bills.Add(objBill);
                    objApplicationDBContext.SaveChanges();

                    // update cache
                    User objUserSession = SessionManager.GetCurrentUser();
                    objUserSession.Bills.Add(objBill);
                    SessionManager.SetCurrentUser(objUserSession);

                    // nulls to avoid json recursion exception
                    // this data will be not used anyway
                    objBill.Shop.UserOwner = null;
                    for (int i = 0; i < objBill.Entries.Count; i++)
                    {
                        objBill.Entries[i].Category.UserOwner = null;
                    }

                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(
                        new Response()
                        {
                            Success = true,
                            Message = "OK",
                            Bill = objBill,
                            NewPriceFormatted = objBill.Entries.Sum(x => x.Price).ToString("C", new System.Globalization.CultureInfo("pl-PL")),
                            NewProductsCount = objBill.Entries.Count
                        }));
                }
                catch (Exception ex)
                {
                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(new Response() { Success = false, Message = ex.ToString() }));
                }
            }
            else if (strAction.Equals("Edit"))
            {
                try
                {
                    ViewBill objViewBill = JsonConvert.DeserializeObject<ViewBill>(strJSON);

                    ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

                    Bill objBill = objApplicationDBContext
                                    .Bills
                                    .Include("Entries")
                                    .Include("Shop")
                                    .Include("Entries.Category")
                                    .Where(x => x.BillID == objViewBill.BillID)
                                    .FirstOrDefault();

                    if (objBill != null)
                    {
                        int nCurrentUserID = SessionManager.GetCurrentUser().UserID;

                        List<ProductCategory> listProductCategory = objApplicationDBContext
                                                                    .ProductCategories
                                                                    .Where(x => x.UserOwnerId == nCurrentUserID)
                                                                    .ToList();

                        objBill.PurchaseDate = objViewBill.PurchaseDate;
                        AssignShopToBill(objApplicationDBContext, ref objBill, objViewBill, nCurrentUserID);

                        // remove deleted entries
                        objBill.Entries.RemoveAll(x => !objViewBill.Products.Select(y => y.ProductID).Contains(x.BillEntryID));

                        for (int i = 0; i < objViewBill.Products.Count; i++)
                        {
                            BillEntry objBillEntryToUpdate = objBill.Entries.Find(x => x.BillEntryID == objViewBill.Products[i].ProductID);

                            if (objBillEntryToUpdate == null)
                            {
                                objBill.Entries.Add(new BillEntry()
                                {
                                    Category = listProductCategory.Find(x => x.ProductCategoryID == objViewBill.Products[i].ProductCategoryID),
                                    ProductName = objViewBill.Products[i].ProductName,
                                    Quantity = objViewBill.Products[i].Quantity,
                                    Price = objViewBill.Products[i].Price,
                                });
                            }
                            else
                            {
                                objBillEntryToUpdate.Category = listProductCategory.Find(x => x.ProductCategoryID == objViewBill.Products[i].ProductCategoryID);
                                objBillEntryToUpdate.ProductName = objViewBill.Products[i].ProductName;
                                objBillEntryToUpdate.Quantity = objViewBill.Products[i].Quantity;
                                objBillEntryToUpdate.Price = objViewBill.Products[i].Price;
                            }
                        }

                        objApplicationDBContext.SaveChanges();

                        // update cache
                        User objUser = SessionManager.GetCurrentUser();
                        int nIndex = objUser.Bills.FindIndex(x => x.BillID == objBill.BillID);
                        objUser.Bills[nIndex] = objBill;
                        SessionManager.SetCurrentUser(objUser);
                    }

                    // nulls to avoid json recursion exception
                    // this data will be not used anyway
                    objBill.Shop.UserOwner = null;
                    for (int i = 0; i < objBill.Entries.Count; i++)
                    {
                        objBill.Entries[i].Category.UserOwner = null;
                    }

                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(new Response()
                    {
                        Success = true,
                        Message = "OK",    
                        Bill = objBill,                    
                        NewPriceFormatted = objBill.Entries.Sum(x => x.Price).ToString("C", new System.Globalization.CultureInfo("pl-PL")),
                        NewProductsCount = objBill.Entries.Count
                    }));
                }
                catch (Exception ex)
                {
                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(new Response() { Success = false, Message = ex.ToString() }));
                }
            }
            else if (strAction.Equals("Get"))
            {
                User objUser = SessionManager.GetCurrentUser();
                int nBillID = Convert.ToInt32(objHttpContext.Request.QueryString["BillID"]);
                Bill objBill = objUser.Bills.Where(x => x.BillID == nBillID).FirstOrDefault();
                objHttpContext.Response.ContentType = "application/json";

                // nulls to avoid json recursion exception
                // this data will be not used anyway
                objBill.Shop.UserOwner = null;
                for (int i = 0; i < objBill.Entries.Count; i++)
                {
                    objBill.Entries[i].Category.UserOwner = null;
                }

                objHttpContext.Response.Write(JsonConvert.SerializeObject(new Response()
                {
                    Success = objBill != null,
                    Bill = objBill
                }
                ));
            }
        }

        public void AssignShopToBill(ApplicationDBContext objApplicationDBContext, ref Bill objBill, ViewBill objViewBill, int nCurrentUserID)
        {
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
            
            objBill.Shop = objShop;
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