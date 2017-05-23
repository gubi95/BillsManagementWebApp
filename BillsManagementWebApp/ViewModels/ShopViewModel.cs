using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.ViewModels
{
    public class ShopViewModel
    {
        public int ShopID { get; set; }
        public string ShopName { get; set; }

        public ShopViewModel(Shop objShop)
        {
            this.ShopID = objShop.ShopID;
            this.ShopName = objShop.ShopName;
        }
    }
}