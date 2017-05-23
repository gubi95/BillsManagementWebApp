using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp
{
    public class Constants
    {
        public static string LoginLayoutPath = "/Views/Shared/_LoginLayout.cshtml";
        public static string BaseLayoutPath = "/Views/Shared/_BaseLayout.cshtml";
        public static string SessionKey_CurrentUser = "CurrUser_SessKey";

        public static ProductCategory GetDefaultProductCategory()
        {
            return new ProductCategory()
            {
                Color = "#cccccc",
                MonthBudget = 0,
                Name = "Bez kategorii",
                ProductCategoryID = -1,
                UserOwner = null,
                UserOwnerId = -1
            };
        }

        public static List<ProductCategory> GetPreDefinedCategories(User objUserOwner)
        {
            List<ProductCategory> listProductCategory = new List<ProductCategory>()
            {
                new ProductCategory() { Color = "#ff0000", MonthBudget = 0, Name = "Jedzenie", UserOwner = objUserOwner, UserOwnerId = objUserOwner.UserID },
                new ProductCategory() { Color = "#00ff00", MonthBudget = 0, Name = "Opłaty", UserOwner = objUserOwner, UserOwnerId = objUserOwner.UserID },
                new ProductCategory() { Color = "#0000ff", MonthBudget = 0, Name = "Rozrywka", UserOwner = objUserOwner, UserOwnerId = objUserOwner.UserID },
                new ProductCategory() { Color = "#ffff00", MonthBudget = 0, Name = "Podróże", UserOwner = objUserOwner, UserOwnerId = objUserOwner.UserID },
                new ProductCategory() { Color = "#00ffff", MonthBudget = 0, Name = "Edukacja", UserOwner = objUserOwner, UserOwnerId = objUserOwner.UserID },
                new ProductCategory() { Color = "#ff00ff", MonthBudget = 0, Name = "Filmy i książki", UserOwner = objUserOwner, UserOwnerId = objUserOwner.UserID }
            };

            return listProductCategory;
        }
    }
}