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
        public List<BillEntryViewModel> Entries { get; set; }

        public BillViewModel() { }

        public BillViewModel(Models.Bill objBill)
        {
            this.ApplyFromModel(objBill);
        }

        public void ApplyFromModel(BillsManagementWebApp.Models.Bill objBill)
        {
            this.BillID = objBill.BillID;
            this.PurchaseDate = objBill.PurchaseDate;

            this.Entries = new List<BillEntryViewModel>();
            if (objBill.Entries != null)
            {
                this.Entries = objBill.Entries.Select(x => new BillEntryViewModel(x)).ToList();
            }
        }

        public void ApplyToModel(ref BillsManagementWebApp.Models.Bill objBill)
        {
            objBill.PurchaseDate = this.PurchaseDate;
        }
    }
}