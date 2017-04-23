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
        public class BillReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0,
                WRONG_PURCHASE_DATE = 1,
                WRONG_USER_ID = 2,
                WRONG_SHOP_ID = 3,
                WRONG_SHOP_NAME = 4,
                WRONG_SHOP_OWNER_ID = 5,
                NO_BILL_ENTRIES = 6,
                WRONG_CATEGORY_ID = 7,
                WRONG_PRICE = 8,
                WRONG_PRODUCT_NAME = 9,
                WRONG_QUANTITY = 10,
                WRONG_BILL_ID
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public BillApiWrapper Bill { get; set; }            

            public BillReturnEntityWrapper(EnumReturnCodes enumRetCode)
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

        public class BillApiWrapper
        {            
            public int UserID { get; set; }
            public int BillID { get; set; }
            public DateTime PurchaseDate { get; set; }             
            public ShopController.ShopApiWrapper Shop { get; set; }
            public List<BillEntryController.BillEntryApiWrapper> Entries { get; set; }            

            public BillApiWrapper() { }
            public BillApiWrapper(Bill objBill)
            {                   
                this.BillID = objBill.BillID;
                this.PurchaseDate = objBill.PurchaseDate;
                this.Entries = new List<BillEntryController.BillEntryApiWrapper>();
                this.Shop = new ShopController.ShopApiWrapper(objBill.Shop);

                foreach (BillEntry objBillEntry in objBill.Entries)
                {
                    this.Entries.Add(new BillEntryController.BillEntryApiWrapper(objBillEntry));
                }
            }

            public void FillModel(ref Bill objBill, bool bFillForUpdate)
            {
                objBill.PurchaseDate = this.PurchaseDate;                

                if (!bFillForUpdate)
                {
                    objBill.Entries = new List<BillEntry>();
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
        public BillReturnEntityWrapper Create(BillApiWrapper objBillApiWrapper)
        {
            if (objBillApiWrapper.PurchaseDate == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_PURCHASE_DATE);
            }

            if (objBillApiWrapper.Entries == null || objBillApiWrapper.Entries.Count == 0)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.NO_BILL_ENTRIES);
            }

            if (objBillApiWrapper.Shop == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_ID);
            }

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Models.User objUser = objApplicationDBContext
                .Users
                .Include("Bills")
                .Where(x => x.UserID == objBillApiWrapper.UserID)
                .FirstOrDefault();

            if (objUser == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_USER_ID);
            }

            List<ProductCategory> listProductCategory = objApplicationDBContext
                                                            .ProductCategories
                                                            .ToList();

            // bill entries validation
            foreach (BillEntryController.BillEntryApiWrapper objBillEntryApiWrapper in objBillApiWrapper.Entries)
            {
                if (objBillEntryApiWrapper.Category == null || listProductCategory.Find(x => x.ProductCategoryID == objBillEntryApiWrapper.Category.ProductCategoryID) == null)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_CATEGORY_ID);
                }

                if (objBillEntryApiWrapper.Price <= 0.0M)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_PRICE);
                }

                if (("" + objBillEntryApiWrapper.ProductName).Trim() == "")
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_PRODUCT_NAME);
                }

                if (objBillEntryApiWrapper.Quantity <= 0.0)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_QUANTITY);
                }
            }

            List<Shop> listShop = objApplicationDBContext
                                    .Shops
                                    .Include("UserOwner")
                                    .Where(x => x.UserOwner.UserID == objUser.UserID)
                                    .ToList();

            Shop objShopToAssign = null;
            if (listShop.Find(x => x.ShopID == objBillApiWrapper.Shop.ShopID && objBillApiWrapper.Shop.UserOwnerID == objUser.UserID) != null)
            {
                objShopToAssign = listShop.Find(x => x.ShopID == objBillApiWrapper.Shop.ShopID && objBillApiWrapper.Shop.UserOwnerID == objUser.UserID);
            }
            else
            {
                if (("" + objBillApiWrapper.Shop.ShopName).Trim() == "")
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_NAME);
                }

                if (objUser.UserID != objBillApiWrapper.Shop.UserOwnerID)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_OWNER_ID);
                }

                objShopToAssign = new Shop()
                {
                    ShopName = ("" + objBillApiWrapper.Shop.ShopName).Trim(),
                    UserOwner = objUser
                };

                objApplicationDBContext.Shops.Add(objShopToAssign);
                objApplicationDBContext.SaveChanges();
            }

            Bill objBill = new Bill();
            objBillApiWrapper.FillModel(ref objBill, false);

            objBill.Shop = objShopToAssign;

            for (int i = 0; i < objBill.Entries.Count; i++)
            {
                objBill.Entries[i].Category = listProductCategory.Find(x => x.ProductCategoryID == objBill.Entries[i].Category.ProductCategoryID);
            }

            objUser.Bills.Add(objBill);
            objApplicationDBContext.SaveChanges();

            return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.OK) { Bill = new BillApiWrapper(objBill) };
        }

        [HttpPost]
        [ActionName("edit")]
        public BillReturnEntityWrapper Edit(BillApiWrapper objBillApiWrapper)
        {   
            if (objBillApiWrapper.PurchaseDate == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_PURCHASE_DATE);
            }

            if (objBillApiWrapper.Entries == null || objBillApiWrapper.Entries.Count == 0)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.NO_BILL_ENTRIES);
            }

            if (objBillApiWrapper.Shop == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_ID);
            }

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Models.User objUser = objApplicationDBContext
                .Users                
                .Where(x => x.UserID == objBillApiWrapper.UserID)
                .FirstOrDefault();

            if (objUser == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_USER_ID);
            }

            List<ProductCategory> listProductCategory = objApplicationDBContext
                                                            .ProductCategories
                                                            .ToList();

            // bill entries validation
            foreach (BillEntryController.BillEntryApiWrapper objBillEntryApiWrapper in objBillApiWrapper.Entries)
            {
                if (objBillEntryApiWrapper.Category == null || listProductCategory.Find(x => x.ProductCategoryID == objBillEntryApiWrapper.Category.ProductCategoryID) == null)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_CATEGORY_ID);
                }

                if (objBillEntryApiWrapper.Price <= 0.0M)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_PRICE);
                }

                if (("" + objBillEntryApiWrapper.ProductName).Trim() == "")
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_PRODUCT_NAME);
                }

                if (objBillEntryApiWrapper.Quantity <= 0.0)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_QUANTITY);
                }
            }

            List<Shop> listShop = objApplicationDBContext
                                    .Shops
                                    .Include("UserOwner")
                                    .Where(x => x.UserOwner.UserID == objUser.UserID)
                                    .ToList();

            Shop objShopToAssign = null;
            if (listShop.Find(x => x.ShopID == objBillApiWrapper.Shop.ShopID && objBillApiWrapper.Shop.UserOwnerID == objUser.UserID) != null)
            {
                objShopToAssign = listShop.Find(x => x.ShopID == objBillApiWrapper.Shop.ShopID && objBillApiWrapper.Shop.UserOwnerID == objUser.UserID);
            }
            else
            {
                if (("" + objBillApiWrapper.Shop.ShopName).Trim() == "")
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_NAME);
                }

                if (objUser.UserID != objBillApiWrapper.Shop.UserOwnerID)
                {
                    return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_OWNER_ID);
                }

                objShopToAssign = new Shop()
                {
                    ShopName = ("" + objBillApiWrapper.Shop.ShopName).Trim(),
                    UserOwner = objUser
                };

                objApplicationDBContext.Shops.Add(objShopToAssign);
                objApplicationDBContext.SaveChanges();
            }

            Bill objBill = objApplicationDBContext
                .Bills                
                .Include("Shop")
                .Include("Shop.UserOwner")
                .Where(x => x.BillID == objBillApiWrapper.BillID)
                .FirstOrDefault();

            if (objBill == null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_BILL_ID);
            }

            objBillApiWrapper.FillModel(ref objBill, true);
            objBill.Shop = objShopToAssign;
            
            objApplicationDBContext.SaveChanges();

            return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.OK);
        }

        [HttpPost]
        [ActionName("delete")]
        public BillReturnEntityWrapper Delete(BillApiWrapper objBillApiWrapper)
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
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.OK);
            }
            else
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_BILL_ID);
            }
        }

        [HttpGet]
        [ActionName("get")]
        public BillReturnEntityWrapper Get(int id)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Bill objBill = new ApplicationDBContext()
                .Bills
                .Include("Shop")
                .Include("Shop.UserOwner")
                .Include("Entries")
                .Include("Entries.Category")
                .Where(x => x.BillID == id)
                .FirstOrDefault();

            if (objBill != null)
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.OK) { Bill = new BillApiWrapper(objBill) };
            }
            else
            {
                return new BillReturnEntityWrapper(BillReturnEntityWrapper.EnumReturnCodes.WRONG_BILL_ID);
            }
        }
    }
}
