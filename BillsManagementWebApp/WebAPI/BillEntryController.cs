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
        public class BillEntryReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0,
                WRONG_BILL_ENTRY_ID = 1,
                WRONG_PRODUCT_NAME = 2,
                WRONG_PRICE = 3,
                WRONG_QUANTITY = 4,
                WRONG_CATEGORY = 5,
                WRONG_BILL_ID = 6
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public BillEntryApiWrapper BillEntry { get; set; }

            public BillEntryReturnEntityWrapper(EnumReturnCodes enumRetCode)
            {
                this.enumReturnCode = enumRetCode;
            }

            public int ReturnCode
            {
                get
                {
                    return (int)this.enumReturnCode;
                }
            }

            public string ReturnMessage
            {
                get
                {
                    return this.enumReturnCode.ToString();
                }
            }
        }

        public class BillEntryApiWrapper
        {
            // additional
            public int BillID { get; set; }         
            public int BillEntryID { get; set; }
            public int ExternalSystemID { get; set; }
            public DateTime LastModifiedDate { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public double Quantity { get; set; }
            public ProductCategoryController.ProductCategoryApiWrapper Category { get; set; }

            public BillEntryApiWrapper() { }
            public BillEntryApiWrapper(BillEntry objBillEntry)
            {
                this.LastModifiedDate = objBillEntry.LastModifiedDate;
                this.ExternalSystemID = objBillEntry.ExternalSystemID;
                this.BillEntryID = objBillEntry.BillEntryID;
                this.ProductName = objBillEntry.ProductName;
                this.Price = objBillEntry.Price;
                this.Quantity = objBillEntry.Quantity;
                if (objBillEntry.Category != null)
                {
                    this.Category = new ProductCategoryController.ProductCategoryApiWrapper(objBillEntry.Category);
                }                
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
        [ActionName("create")]
        public BillEntryReturnEntityWrapper Create(List<BillEntryApiWrapper> listBillEntryApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            List<ProductCategory> listProductCategory = objApplicationDBContext
                .ProductCategories
                .ToList();

            List<Bill> listBill = objApplicationDBContext
                                    .Bills
                                    .Include("Entries")
                                    .ToList();            

            foreach (BillEntryApiWrapper objBillEntryApiWrapper in listBillEntryApiWrapper)
            {
                if (("" + objBillEntryApiWrapper.ProductName).Trim() == "")
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_PRODUCT_NAME);
                }

                if (objBillEntryApiWrapper.Price <= 0.0M)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_PRICE);
                }

                if (objBillEntryApiWrapper.Quantity <= 0.0)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_QUANTITY);
                }

                if (objBillEntryApiWrapper.Category == null ||
                    listProductCategory.Find(x => x.ProductCategoryID == objBillEntryApiWrapper.Category.ProductCategoryID) == null)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_CATEGORY);
                }

                if (listBill.Find(x => x.BillID == objBillEntryApiWrapper.BillID) == null)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_BILL_ID);
                }
            }

            foreach (BillEntryApiWrapper objBillEntryApiWrapper in listBillEntryApiWrapper)
            {
                Bill objBill = listBill.Find(x => x.BillID == objBillEntryApiWrapper.BillID);

                objBill.Entries.Add(new BillEntry()
                {                       
                    Category = listProductCategory.Find(x => x.ProductCategoryID == objBillEntryApiWrapper.Category.ProductCategoryID),
                    Price = objBillEntryApiWrapper.Price,
                    ProductName = ("" + objBillEntryApiWrapper.ProductName).Trim(),
                    Quantity = objBillEntryApiWrapper.Quantity
                });
            }                                     
            
            objApplicationDBContext.SaveChanges();
            return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.OK);
        }

        [HttpPost]
        [ActionName("edit")]
        public BillEntryReturnEntityWrapper Edit(List<BillEntryApiWrapper> listBillEntryApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            List<ProductCategory> listProductCategory = objApplicationDBContext
                .ProductCategories
                .ToList();

            List<int> listIDs = listBillEntryApiWrapper.Select(y => y.BillEntryID).ToList();

            List<BillEntry> listBillEntry = objApplicationDBContext
                .BillEntries
                .Include("Category")
                .Where(x => listIDs.Contains(x.BillEntryID))
                .ToList();

            if (listBillEntry.Count != listBillEntryApiWrapper.Count)
            {
                return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_BILL_ENTRY_ID);
            }

            foreach (BillEntryApiWrapper objBillEntryApiWrapper in listBillEntryApiWrapper)
            {
                if (("" + objBillEntryApiWrapper.ProductName).Trim() == "")
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_PRODUCT_NAME);
                }

                if (objBillEntryApiWrapper.Price <= 0.0M)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_PRICE);
                }

                if (objBillEntryApiWrapper.Quantity <= 0.0)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_QUANTITY);
                }

                if (objBillEntryApiWrapper.Category == null ||
                    listProductCategory.Find(x => x.ProductCategoryID == objBillEntryApiWrapper.Category.ProductCategoryID) == null)
                {
                    return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.WRONG_CATEGORY);
                }
            }

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

            return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.OK);
        }

        [HttpPost]
        [ActionName("delete")]
        public BillEntryReturnEntityWrapper Delete(List<int> IDs)
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

            return new BillEntryReturnEntityWrapper(BillEntryReturnEntityWrapper.EnumReturnCodes.OK);
        }
    }
} 