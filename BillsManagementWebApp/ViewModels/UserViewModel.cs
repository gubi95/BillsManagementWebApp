﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.Models;

namespace BillsManagementWebApp.ViewModels
{
    public class UserViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<BillViewModel> Bills { get; set; }

        public string UserDisplayNameFormatted
        {
            get
            {
                return this.FirstName + " " + this.LastName;
            }
        }

        public UserViewModel() { }

        public UserViewModel(User objUser)
        {
            this.ApplyFromModel(objUser);
        }

        public void ApplyFromModel(User objUser)
        {
            this.Username = objUser.Username;
            this.Password = objUser.Password;
            this.Email = objUser.Email;
            this.FirstName = objUser.FirstName;
            this.LastName = objUser.LastName;

            this.Bills = new List<BillViewModel>();
            if (objUser.Bills != null)
            {
                this.Bills = objUser.Bills.Select(x => new BillViewModel(x)).ToList();
            }
        }

        public void ApplyToModel(ref User objUser)
        {
            objUser.Username = this.Username;
            objUser.Password = this.Password;
            objUser.Email = this.Email;
            objUser.FirstName = this.FirstName;
            objUser.LastName = this.LastName;
        }
    }
}