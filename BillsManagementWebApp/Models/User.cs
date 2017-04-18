using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BillsManagementWebApp.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }
        
        [Required]
        [MaxLength(40)]
        public string Username { get; set; }

        [Required]
        [MaxLength(300)]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(150)]
        public string LastName { get; set; }

        public List<Bill> Bills { get; set; }
    }
}