﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace BillsManagementWebApp.Models
{
    public class Bill
    {
        [Key]
        public int BillID { get; set; }

        [Required]
        [Column(TypeName = "Date")]        
        public DateTime PurchaseDate { get; set; }

        public List<BillEntry> Entries { get; set; }

        [Required]                    
        public Shop Shop { get; set; }

        [Required]
        public DateTime LastModifiedDate { get; set; }

        public int ExternalSystemID { get; set; }
    }
}