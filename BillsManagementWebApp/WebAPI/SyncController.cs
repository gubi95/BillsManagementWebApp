using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.WebAPI
{
    public class SyncController : ApiController
    {
        public class SyncReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0, 
                WRONG_USER_ID = 1               
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public List<BillController.BillApiWrapper> Bills { get; set; }
            public List<ProductCategoryController.ProductCategoryApiWrapper> Categories { get; set; }
            public List<ShopController.ShopApiWrapper> Shops { get; set; }

            public SyncReturnEntityWrapper(EnumReturnCodes enumRetCode)
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

        [HttpGet]
        [ActionName("getall")]
        public SyncReturnEntityWrapper GetAll(int UserID)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Models.User objUser = objApplicationDBContext
                                    .Users
                                    .Include("Bills")
                                    .Include("Bills.Entries")
                                    .Where(x => x.UserID == UserID)
                                    .FirstOrDefault();

            if (objUser == null)
            {
                return new SyncReturnEntityWrapper(SyncReturnEntityWrapper.EnumReturnCodes.WRONG_USER_ID);
            }

            List<Shop> listShop = objApplicationDBContext
                                    .Shops
                                    .Include("UserOwner")
                                    .Where(x => x.UserOwner.UserID == UserID)
                                    .ToList();

            List<ProductCategory> listProductCategory = objApplicationDBContext
                                                            .ProductCategories
                                                            .Where(x => x.UserOwnerId == UserID)
                                                            .ToList();

            List<BillController.BillApiWrapper> listBillApiWrapper = objUser.Bills.Select(x => new BillController.BillApiWrapper(x)).ToList();
            List<ShopController.ShopApiWrapper> listShopApiWrapper = listShop.Select(x => new ShopController.ShopApiWrapper(x)).ToList();
            List<ProductCategoryController.ProductCategoryApiWrapper> listProductCategoryApiWrapper = listProductCategory.Select(x => new ProductCategoryController.ProductCategoryApiWrapper(x)).ToList();

            return new SyncReturnEntityWrapper(SyncReturnEntityWrapper.EnumReturnCodes.OK)
            {
                Bills = listBillApiWrapper,
                Categories = listProductCategoryApiWrapper,
                Shops = listShopApiWrapper
            };
        }
    }
}
