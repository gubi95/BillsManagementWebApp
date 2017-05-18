using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BillsManagementWebApp.Models
{
    public class ProductCategory
    {
        [Key]
        public int ProductCategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public string Color { get; set; }

        public decimal MonthBudget { get; set; }

        [Required]
        public int UserOwnerId { get; set; }

        [Required]
        [ForeignKey("UserOwnerId")]
        public User UserOwner { get; set; }
    }
}