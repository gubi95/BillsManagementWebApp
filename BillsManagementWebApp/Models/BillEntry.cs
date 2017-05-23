using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BillsManagementWebApp.Models
{
    public class BillEntry
    {
        [Key]
        public int BillEntryID { get; set; }

        [MaxLength(100)]
        [Required]
        public string ProductName { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public double Quantity { get; set; }
                        
        public ProductCategory Category { get; set; }
    }
}