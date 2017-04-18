using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BillsManagementWebApp.Models;
using System.Web.Http;

namespace BillsManagementWebApp.WebAPI
{
    public class UserController : ApiController
    {
        public class UserApiWrapper
        {
            public int UserID { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }            
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; } 
            public List<BillController.BillApiWrapper> Bills { get; set; }

            public UserApiWrapper() { }
            public UserApiWrapper(Models.User objUser)
            {
                this.UserID = objUser.UserID;
                this.Username = objUser.Username;
                this.Password = objUser.Password;
                this.Email = objUser.Email;
                this.FirstName = objUser.FirstName;
                this.LastName = objUser.LastName;
                this.Bills = new List<BillController.BillApiWrapper>();

                foreach (Bill objBill in objUser.Bills)
                {
                    this.Bills.Add(new BillController.BillApiWrapper(objBill));
                }         
            }

            public void FillModel(ref Models.User objUser)
            {
                objUser.Username = this.Username;

                if (objUser.Password != this.Password)
                {
                    objUser.Password = BillsManagementWebApp.Shared.PasswordHasher.GenerateHashForUser(this.Password);
                }

                objUser.Email = this.Email;
                objUser.FirstName = this.FirstName;
                objUser.LastName = this.LastName;
            }
        }

        [HttpPost]
        [ActionName("login")]
        public UserApiWrapper Login(UserApiWrapper objUserApiWrapper)
        {
            Models.User objUser = new ApplicationDBContext()
                .Users
                .Include("Bills")
                .Include("Bills.Entries")
                .Include("Bills.Entries.Category")
                .Where(x => x.Username == objUserApiWrapper.Username)
                .FirstOrDefault();

            if (objUser != null)
            {
                string strHashedPassword = objUser.Password;
                string strPassword = objUserApiWrapper.Password;

                bool bPasswordMatches = BillsManagementWebApp.Shared.PasswordHasher.CompareHashes(strPassword, strHashedPassword);

                if (bPasswordMatches)
                {
                    return new UserApiWrapper(objUser);
                }
            }

            return null;
        } 

        [HttpPost]
        [ActionName("create")]
        public int Create(UserApiWrapper objUserApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Models.User objUser = new Models.User()
            {
                Email = objUserApiWrapper.Email,
                FirstName = objUserApiWrapper.FirstName,
                LastName = objUserApiWrapper.LastName,
                Password = BillsManagementWebApp.Shared.PasswordHasher.GenerateHashForUser(objUserApiWrapper.Password),
                Username = objUserApiWrapper.Username
            };

            objApplicationDBContext.Users.Add(objUser);
            objApplicationDBContext.SaveChanges();

            return objUser.UserID;
        }

        [HttpPost]
        [ActionName("edit")]
        public void Edit(UserApiWrapper objUserApiWrapper)
        {
            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Models.User objUser = objApplicationDBContext.Users.Where(x => x.UserID == objUserApiWrapper.UserID).FirstOrDefault();

            if (objUser != null)
            {
                objUserApiWrapper.FillModel(ref objUser);
                objApplicationDBContext.SaveChanges();
            }
        }
    }
}