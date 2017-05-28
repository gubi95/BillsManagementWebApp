using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System.IO;
using BillsManagementWebApp.Models;
using BillsManagementWebApp.Shared;
using System.Web.SessionState;

namespace BillsManagementWebApp.Handlers
{
    /// <summary>
    /// Summary description for ChartsDataHandler
    /// </summary>
    public class ChartsDataHandler : IHttpHandler, IRequiresSessionState
    {
        private class PostedData
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string ChartType { get; set; }
            public bool IsBarChartWithMonthBudget { get; set; }
        }

        private class CategoryPieChartSeries
        {
            public string CategoryName { get; set; }
            public string CategoryColor { get; set; }
            public decimal Price { get; set; }
            public decimal MonthBudget { get; set; } 

            public CategoryPieChartSeries(ProductCategoryToTotalPrice objProductCategoryToTotalPrice)
            {
                this.CategoryName = objProductCategoryToTotalPrice.Category.Name;
                this.CategoryColor = objProductCategoryToTotalPrice.Category.Color;
                this.Price = objProductCategoryToTotalPrice.Price;
                this.MonthBudget = objProductCategoryToTotalPrice.Category.MonthBudget;
            }
        }

        private class ProductCategoryToTotalPrice
        {
            public ProductCategory Category { get; set; }
            public decimal Price { get; set; }
        }

        public void ProcessRequest(HttpContext objHttpContext)
        {
            string strJSON = new StreamReader(objHttpContext.Request.InputStream).ReadToEnd();

            PostedData objPostedData = JsonConvert.DeserializeObject<PostedData>(strJSON);

            if (objPostedData != null)
            {
                if (objPostedData.ChartType == "CategoryBarChart")
                { 
                    Models.User objUser = SessionManager.GetCurrentUser();

                    List<int> listBillIDs = objUser.Bills.Select(y => y.BillID).ToList();
                                                                                
                    List<Bill> listBill = new ApplicationDBContext()
                                            .Bills
                                            .Include("Entries")
                                            .Include("Entries.Category")
                                            .Where(
                                                x => listBillIDs.Contains(x.BillID) &&
                                                x.PurchaseDate >= objPostedData.From &&
                                                x.PurchaseDate <= objPostedData.To)
                                            .ToList();

                    List<ProductCategoryToTotalPrice> listProductCategoryToTotalPrice = new List<ProductCategoryToTotalPrice>();

                    foreach (Bill objBill in listBill)
                    {
                        foreach (BillEntry objBillEntry in objBill.Entries)
                        {
                            int nCategoryIdToCompare = objBillEntry.Category != null ? objBillEntry.Category.ProductCategoryID : -1;
                            int nIndex = listProductCategoryToTotalPrice.FindIndex(x => x.Category.ProductCategoryID == nCategoryIdToCompare);

                            if (nIndex >= 0)
                            {
                                listProductCategoryToTotalPrice[nIndex].Price += objBillEntry.Price;
                            }
                            else
                            {
                                listProductCategoryToTotalPrice.Add(new ProductCategoryToTotalPrice()
                                {
                                    Category = objBillEntry.Category != null ? objBillEntry.Category : Constants.GetDefaultProductCategory(),
                                    Price = objBillEntry.Price });
                            }
                        }
                    }

                    List<CategoryPieChartSeries> listCategoryPieChartSeries = listProductCategoryToTotalPrice
                                                                                .OrderBy(x => x.Category.Name)
                                                                                .Select(x => new CategoryPieChartSeries(x)).ToList();

                    objHttpContext.Response.ContentType = "application/json";
                    objHttpContext.Response.Write(JsonConvert.SerializeObject(listCategoryPieChartSeries));
                    objHttpContext.Response.Flush();
                    objHttpContext.Response.End();
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