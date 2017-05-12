using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.WebAPI
{
    public class ProductCategoryController : ApiController
    {
        public class ProductcategoryReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0                
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public List<ProductCategoryApiWrapper> Categories { get; set; }

            public ProductcategoryReturnEntityWrapper(EnumReturnCodes enumRetCode)
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

        public class ProductCategoryApiWrapper
        {
            public int ProductCategoryID { get; set; }
            public string Name { get; set; }

            public ProductCategoryApiWrapper() { }
            public ProductCategoryApiWrapper(ProductCategory objProductCategory)
            {
                this.ProductCategoryID = objProductCategory.ProductCategoryID;
                this.Name = objProductCategory.Name;
            }

            public void FillModel(ref ProductCategory objProductCategory)
            {
                objProductCategory.ProductCategoryID = this.ProductCategoryID;
                objProductCategory.Name = this.Name;
            }
        }

        [HttpGet]
        [ActionName("getall")]
        public ProductcategoryReturnEntityWrapper GetAll(int UserID)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            List<ProductCategory> listProductCategory = objApplicationDBContext
                .ProductCategories
                .Where(x => x.UserOwnerId == UserID)
                .ToList();

            List<ProductCategoryApiWrapper> listProductCategoryApiWrapper = new List<ProductCategoryApiWrapper>();

            foreach (ProductCategory objProductCategory in listProductCategory)
            {
                listProductCategoryApiWrapper.Add(new ProductCategoryApiWrapper(objProductCategory));
            }

            return new ProductcategoryReturnEntityWrapper(ProductcategoryReturnEntityWrapper.EnumReturnCodes.OK) { Categories = listProductCategoryApiWrapper };
        }
    }
}
