using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BillsManagementWebApp.ViewModels
{
    public class BillViewModel
    {
        public int BillID { get; set; }  
        public DateTime PurchaseDate { get; set; }

        public void ApplyFromModel(BillsManagementWebApp.Models.Bill objBill)
        {
            this.BillID = objBill.BillID;
            this.PurchaseDate = objBill.PurchaseDate;
        }

        public void ApplyToModel(ref BillsManagementWebApp.Models.Bill objBill)
        {
            objBill.PurchaseDate = this.PurchaseDate;
        }
    }
}