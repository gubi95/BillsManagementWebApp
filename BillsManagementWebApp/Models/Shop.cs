using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BillsManagementWebApp.Models
{
    public class Shop
    {
        [Key]
        public int ShopID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ShopName { get; set; }

        [Required]
        public User UserOwner { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        public int ExternalSystemID { get; set; }
    }
}