using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.ViewModels
{
    public class BillEntryViewModel
    {
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public decimal Price { get; set; }

        public BillEntryViewModel(BillEntry objBillEntry)
        {
            this.ProductName = objBillEntry.ProductName;
            this.Quantity = objBillEntry.Quantity;
            this.Price = objBillEntry.Price;
        }
    }
}