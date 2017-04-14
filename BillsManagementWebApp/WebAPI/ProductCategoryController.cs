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
        public List<ProductCategoryApiWrapper> GetAll()
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            List<ProductCategory> listProductCategory = objApplicationDBContext.ProductCategories.ToList();

            List<ProductCategoryApiWrapper> listProductCategoryApiWrapper = new List<ProductCategoryApiWrapper>();

            foreach (ProductCategory objProductCategory in listProductCategory)
            {
                listProductCategoryApiWrapper.Add(new ProductCategoryApiWrapper(objProductCategory));
            }

            return listProductCategoryApiWrapper;
        }
    }
}
