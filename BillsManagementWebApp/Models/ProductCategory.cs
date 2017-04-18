using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}