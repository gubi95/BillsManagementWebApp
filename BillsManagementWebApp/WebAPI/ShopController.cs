using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.WebAPI
{
    public class ShopController : ApiController
    {
        public class ShopReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0,
                WRONG_SHOP_ID = 1,
                WRONG_SHOP_NAME = 2,
                WRONG_USER_OWNER_ID = 3
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public ShopApiWrapper Shop { get; set; }
            public List<ShopApiWrapper> Shops { get; set; }

            public ShopReturnEntityWrapper(EnumReturnCodes enumRetCode)
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

        public class ShopApiWrapper
        {
            public int ShopID { get; set; }
            public int ExternalSystemID { get; set; }
            public DateTime LastModifiedDate { get; set; }
            public string ShopName { get; set; }
            public int UserOwnerID { get; set; }

            public ShopApiWrapper() { }
            public ShopApiWrapper(Shop objShop)
            {
                this.ShopID = objShop.ShopID;
                this.ShopName = objShop.ShopName;
                this.UserOwnerID = objShop.UserOwner.UserID;
                this.ExternalSystemID = objShop.ExternalSystemID;
                this.LastModifiedDate = objShop.LastModifiedDate;
            }

            public void FillModel(ref Shop objShop)
            {
                objShop.ShopName = this.ShopName;
            }
        }

        [HttpPost]
        [ActionName("create")]
        public ShopReturnEntityWrapper Create(ShopApiWrapper objShopApiWrapper)
        {
            if (("" + objShopApiWrapper.ShopName).Trim() == "")
            {
                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_NAME);
            }

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            User objUser = objApplicationDBContext
                            .Users
                            .Where(x => x.UserID == objShopApiWrapper.UserOwnerID)
                            .FirstOrDefault();

            if (objUser != null)
            {
                Shop objShop = new Shop()
                {
                    ShopName = objShopApiWrapper.ShopName,
                    UserOwner = objUser
                };

                objApplicationDBContext.Shops.Add(objShop);
                objApplicationDBContext.SaveChanges();

                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.OK) { Shop = new ShopApiWrapper(objShop) };
            }
            else
            {
                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.WRONG_USER_OWNER_ID);
            }
        }

        [HttpPost]
        [ActionName("edit")]
        public ShopReturnEntityWrapper Edit(ShopApiWrapper objShopApiWrapper)
        {
            if (("" + objShopApiWrapper.ShopName).Trim() == "")
            {
                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_NAME);
            }

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Shop objShop = objApplicationDBContext
                            .Shops
                            .Where(x => x.ShopID == objShopApiWrapper.ShopID)
                            .FirstOrDefault();

            if (objShop == null)
            {
                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_ID);
            }

            User objUser = objApplicationDBContext
                            .Users
                            .Where(x => x.UserID == objShopApiWrapper.UserOwnerID)
                            .FirstOrDefault();

            if (objUser != null)
            {
                objShop.ShopName = ("" + objShopApiWrapper.ShopName).Trim();
                objShop.UserOwner = objUser;

                objApplicationDBContext.SaveChanges();

                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.OK);
            }
            else
            {
                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.WRONG_USER_OWNER_ID);
            }
        }

        [HttpPost]
        [ActionName("delete")]
        public ShopReturnEntityWrapper Delete(int id)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Shop objShop = objApplicationDBContext
                            .Shops
                            .Where(x => x.ShopID == id)
                            .FirstOrDefault();

            if (objShop == null)
            {
                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.WRONG_SHOP_ID);
            }
            else
            {
                objApplicationDBContext
                    .Shops
                    .Remove(objShop);

                objApplicationDBContext.SaveChanges();

                return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.OK);
            }
        }

        [HttpGet]
        [ActionName("getall")]
        public ShopReturnEntityWrapper GetAll(int userID)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            List<Shop> listShop = objApplicationDBContext
                                    .Shops
                                    .Include("UserOwner")
                                    .Where(x => x.UserOwner.UserID == userID)
                                    .ToList();

            List<ShopApiWrapper> listShopApiWrapper = new List<ShopApiWrapper>();

            foreach (Shop objShop in listShop)
            {
                listShopApiWrapper.Add(new ShopApiWrapper(objShop));
            }

            return new ShopReturnEntityWrapper(ShopReturnEntityWrapper.EnumReturnCodes.OK) { Shops = listShopApiWrapper };            
        }
    }
}  