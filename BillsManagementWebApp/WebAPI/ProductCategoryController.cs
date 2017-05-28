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
        public class ProductCategoryReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0,
                WRONG_CATEGORY_NAME = 1,
                WRONG_USER_ID = 2,
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public List<ProductCategoryApiWrapper> Categories { get; set; }
            public List<int> NewIDsList { get; set; }

            public ProductCategoryReturnEntityWrapper(EnumReturnCodes enumRetCode)
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
            public string Color { get; set; }
            public decimal MonthBudget { get; set; }
            public int ExternalSystemID { get; set; }
            public DateTime LastModifiedDate { get; set; }

            public ProductCategoryApiWrapper() { }
            public ProductCategoryApiWrapper(ProductCategory objProductCategory)
            {
                this.ProductCategoryID = objProductCategory.ProductCategoryID;
                this.Name = objProductCategory.Name;
                this.Color = objProductCategory.Color;
                this.MonthBudget = objProductCategory.MonthBudget;
                this.ExternalSystemID = objProductCategory.ExternalSystemID;
                this.LastModifiedDate = objProductCategory.LastModifiedDate;
            }

            public void FillModel(ref ProductCategory objProductCategory)
            {
                objProductCategory.ProductCategoryID = this.ProductCategoryID;
                objProductCategory.Name = this.Name;
                objProductCategory.Color = this.Color;
                objProductCategory.MonthBudget = this.MonthBudget;
            }
        }

        [HttpGet]
        [ActionName("getall")]
        public ProductCategoryReturnEntityWrapper GetAll(int UserID)
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

            return new ProductCategoryReturnEntityWrapper(ProductCategoryReturnEntityWrapper.EnumReturnCodes.OK) { Categories = listProductCategoryApiWrapper };
        }

        public class SaveAllPostDataWrapper
        {            
            public int UserID { get; set; }
            public ProductCategoryApiWrapper[] ProductCategories { get; set; }
        }

        [HttpPost]
        [ActionName("saveall")]
        public ProductCategoryReturnEntityWrapper SaveAll(SaveAllPostDataWrapper objSaveAllPostDataWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            int UserID = 1;

            List<ProductCategory> listProductCategory = objApplicationDBContext
                                                            .ProductCategories
                                                            .Where(x => x.UserOwnerId == UserID)
                                                            .ToList();

            User objUser = objApplicationDBContext
                                    .Users
                                    .Where(x => x.UserID == UserID)
                                    .FirstOrDefault();

            List<int> listID = objSaveAllPostDataWrapper.ProductCategories.Select(x => x.ProductCategoryID).ToList();
            List<ProductCategory> listProductCategoryToDelete = listProductCategory.FindAll(x => listID.Contains(x.ProductCategoryID) == false);
            List<int> listProductCategoriesToDeleteIDs = listProductCategoryToDelete.Select(x => x.ProductCategoryID).ToList();

            // set null to BillEntry.Category
            List<BillEntry> listBillEntriesToNullifyCategory = objApplicationDBContext
                .BillEntries
                .Include("Category")
                .Where(x => x.Category != null && listProductCategoriesToDeleteIDs.Contains(x.Category.ProductCategoryID))
                .ToList();

            for (int i = 0; i < listBillEntriesToNullifyCategory.Count; i++)
            {
                listBillEntriesToNullifyCategory[i].Category = null;
                listBillEntriesToNullifyCategory[i].LastModifiedDate = DateTime.Now;
            }
            objApplicationDBContext.SaveChanges();


            List<ProductCategory> listProductCategoryNew = new List<ProductCategory>();


            foreach (ProductCategory objProductCategoryToDelete in listProductCategoryToDelete)
            {
                objApplicationDBContext.ProductCategories.Remove(objProductCategoryToDelete);
            }

            if (objUser == null)
            {
                return new ProductCategoryReturnEntityWrapper(ProductCategoryReturnEntityWrapper.EnumReturnCodes.WRONG_USER_ID);   
            }

            foreach (ProductCategoryApiWrapper objProductCategoryApiWrapper in objSaveAllPostDataWrapper.ProductCategories)
            {
                if (("" + objProductCategoryApiWrapper.Name).Trim() == "")
                {
                    return new ProductCategoryReturnEntityWrapper(ProductCategoryReturnEntityWrapper.EnumReturnCodes.WRONG_CATEGORY_NAME);
                }

                ProductCategory objProductCategory = listProductCategory.Find(x => x.ProductCategoryID == objProductCategoryApiWrapper.ProductCategoryID);

                if (objProductCategory == null)
                {
                    ProductCategory objProductCategoryNew = new ProductCategory();
                    objProductCategoryApiWrapper.FillModel(ref objProductCategoryNew);
                    objProductCategoryNew.ProductCategoryID = 0;
                    objProductCategoryNew.UserOwner = objUser;
                    objProductCategoryNew.UserOwnerId = objUser.UserID;
                    objProductCategoryNew.LastModifiedDate = DateTime.Now;
                    listProductCategoryNew.Add(objProductCategoryNew);                                        
                }
                else
                {
                    objProductCategoryApiWrapper.FillModel(ref objProductCategory);
                    objProductCategory.LastModifiedDate = DateTime.Now;
                }
            }

            objApplicationDBContext.ProductCategories.AddRange(listProductCategoryNew);

            objApplicationDBContext.SaveChanges();
            return new ProductCategoryReturnEntityWrapper(ProductCategoryReturnEntityWrapper.EnumReturnCodes.OK) { NewIDsList = listProductCategoryNew.Select(x => x.ProductCategoryID).ToList() };
        }
    }
}