using BillsManagementWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BillsManagementWebApp.WebAPI
{
    public class BillEntryController : ApiController
    {
        public class BillEntryApiWrapper
        {
            public int BillEntryID { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public double Quantity { get; set; }
            public ProductCategoryController.ProductCategoryApiWrapper Category { get; set; }

            public BillEntryApiWrapper() { }
            public BillEntryApiWrapper(BillEntry objBillEntry)
            {
                this.BillEntryID = objBillEntry.BillEntryID;
                this.ProductName = objBillEntry.ProductName;
                this.Price = objBillEntry.Price;
                this.Quantity = objBillEntry.Quantity;
                this.Category = new ProductCategoryController.ProductCategoryApiWrapper(objBillEntry.Category);
            }

            public void FillModel(ref BillEntry objBillEntry)
            {
                objBillEntry.ProductName = this.ProductName;
                objBillEntry.Price = this.Price;
                objBillEntry.Quantity = this.Quantity;

                ProductCategory objProductCategory = new ProductCategory();
                this.Category.FillModel(ref objProductCategory);
                objBillEntry.Category = objProductCategory;
            }
        }

        [HttpPost]
        [ActionName("edit")]
        public void Edit(List<BillEntryApiWrapper> listBillEntryApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            List<int> listIDs = listBillEntryApiWrapper.Select(y => y.BillEntryID).ToList();

            List<BillEntry> listBillEntry = objApplicationDBContext
                .BillEntries
                .Include("Category")
                .Where(x => listIDs.Contains(x.BillEntryID))
                .ToList();

            List<ProductCategory> listProductCategory = objApplicationDBContext
                .ProductCategories
                .ToList();

            for (int i = 0; i < listBillEntry.Count; i++)
            {
                BillEntryApiWrapper objBillEntryApiWrapper = listBillEntryApiWrapper.Find(x => x.BillEntryID == listBillEntry[i].BillEntryID);

                if (objBillEntryApiWrapper != null)
                {
                    listBillEntry[i].Price = objBillEntryApiWrapper.Price;
                    listBillEntry[i].Quantity = objBillEntryApiWrapper.Quantity;
                    listBillEntry[i].ProductName = objBillEntryApiWrapper.ProductName;
                    listBillEntry[i].Category = listProductCategory.Find(x => x.ProductCategoryID == objBillEntryApiWrapper.Category.ProductCategoryID);                    
                }
            }

            objApplicationDBContext.SaveChanges();
        }

        [HttpPost]
        [ActionName("delete")]
        public void Delete(List<int> IDs)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            objApplicationDBContext
                .BillEntries
                .RemoveRange(
                    objApplicationDBContext
                    .BillEntries
                    .Where(x => IDs.Contains(x.BillEntryID))
                 );

            objApplicationDBContext.SaveChanges();
        }
    }
} 