using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.WebAPI
{
    public class BillController : ApiController
    {
        public class BillApiWrapper
        {            
            public int UserID { get; set; }
            public int BillID { get; set; }
            public DateTime PurchaseDate { get; set; }

            public List<BillEntryController.BillEntryApiWrapper> Entries { get; set; }

            public BillApiWrapper() { }
            public BillApiWrapper(Bill objBill)
            {
                this.BillID = objBill.BillID;
                this.PurchaseDate = objBill.PurchaseDate;
                this.Entries = new List<BillEntryController.BillEntryApiWrapper>();

                foreach (BillEntry objBillEntry in objBill.Entries)
                {
                    this.Entries.Add(new BillEntryController.BillEntryApiWrapper(objBillEntry));
                }
            }

            public void FillModel(ref Bill objBill, bool bFillForUpdate)
            {
                objBill.PurchaseDate = this.PurchaseDate;
                objBill.Entries = new List<BillEntry>();

                if (!bFillForUpdate)
                {
                    foreach (BillEntryController.BillEntryApiWrapper objBillEntryApiWrapper in this.Entries)
                    {
                        BillEntry objBillEntry = new BillEntry();
                        objBillEntryApiWrapper.FillModel(ref objBillEntry);
                        objBill.Entries.Add(objBillEntry);
                    }
                }
            }
        }

        [HttpPost]
        [ActionName("create")]
        public int Create(BillApiWrapper objBillApiWrapper)
        {
            Bill objBill = new Bill();
            objBillApiWrapper.FillModel(ref objBill, false);

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Models.User objUser = objApplicationDBContext
                .Users
                .Include("Bills")
                .Where(x => x.UserID == objBillApiWrapper.UserID)
                .FirstOrDefault();

            List<ProductCategory> listProductCategory = objApplicationDBContext.ProductCategories.ToList();

            if (objUser != null)
            {
                for (int i = 0; i < objBill.Entries.Count; i++)
                {
                    objBill.Entries[i].Category = listProductCategory.Find(x => x.ProductCategoryID == objBill.Entries[i].Category.ProductCategoryID);   
                }

                objUser.Bills.Add(objBill);
                objApplicationDBContext.SaveChanges();
            }

            return objBill.BillID;
        }

        [HttpPost]
        [ActionName("edit")]
        public void Edit(BillApiWrapper objBillApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Bill objBill = objApplicationDBContext
                .Bills
                .Where(x => x.BillID == objBillApiWrapper.BillID)
                .FirstOrDefault();

            if (objBill != null)
            {
                objBillApiWrapper.FillModel(ref objBill, true);
                objApplicationDBContext.SaveChanges();
            }
        }

        [HttpPost]
        [ActionName("delete")]
        public void Delete(BillApiWrapper objBillApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Bill objBill = objApplicationDBContext
                .Bills
                .Include("Entries")
                .Where(x => x.BillID == objBillApiWrapper.BillID)
                .FirstOrDefault();

            if (objBill != null)
            {
                objApplicationDBContext.BillEntries.RemoveRange(objBill.Entries);
                objApplicationDBContext.Bills.Remove(objBill);
                objApplicationDBContext.SaveChanges();
            }
        }

        [HttpGet]
        [ActionName("get")]
        public BillApiWrapper Get(int id)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Bill objBill = new ApplicationDBContext()
                .Bills
                .Include("Entries")
                .Include("Entries.Category")
                .Where(x => x.BillID == id)
                .FirstOrDefault();

            if (objBill != null)
            {
                return new BillApiWrapper(objBill);
            }

            return null;
        }
    }
}
