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
        public class UserReturnEntityWrapper
        {
            public enum EnumReturnCodes
            {
                OK = 0,
                WRONG_USER_ID = 1,
                WRONG_USERNAME_OR_PASSWORD = 2,
                WRONG_USERNAME = 3,
                WRONG_PASSWORD = 4,
                WRONG_EMAIL = 5,
                WRONG_FIRST_NAME = 6,
                WRONG_LAST_NAME = 7
            }

            private EnumReturnCodes enumReturnCode = EnumReturnCodes.OK;

            public UserApiWrapper User { get; set; }

            public UserReturnEntityWrapper(EnumReturnCodes enumRetCode)
            {
                this.enumReturnCode = enumRetCode;
            }

            public int ReturnCode
            {
                get
                {
                    return (int)this.enumReturnCode;
                }
            }

            public string ReturnMessage
            {
                get
                {
                    return this.enumReturnCode.ToString();
                }  
            }
        }

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
        public UserReturnEntityWrapper Login(UserApiWrapper objUserApiWrapper)
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
                    return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.OK) { User = new UserApiWrapper(objUser) };
                }
                else
                {
                    return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_USERNAME_OR_PASSWORD);
                }
            }
            else
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_USERNAME_OR_PASSWORD);
            }
        } 

        [HttpPost]
        [ActionName("create")]
        public UserReturnEntityWrapper Create(UserApiWrapper objUserApiWrapper)
        {
            if (("" + objUserApiWrapper.Username).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_USERNAME);
            }

            if (("" + objUserApiWrapper.Password).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_PASSWORD);
            }

            if (("" + objUserApiWrapper.FirstName).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_FIRST_NAME);
            }

            if (("" + objUserApiWrapper.LastName).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_LAST_NAME);
            }

            if (("" + objUserApiWrapper.Email).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_EMAIL);
            }

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();
            Models.User objUser = new Models.User()
            {
                Email = objUserApiWrapper.Email,
                FirstName = objUserApiWrapper.FirstName,
                LastName = objUserApiWrapper.LastName,
                Password = BillsManagementWebApp.Shared.PasswordHasher.GenerateHashForUser(objUserApiWrapper.Password),
                Username = objUserApiWrapper.Username,
                Bills = new List<Bill>()
            };

            objApplicationDBContext.Users.Add(objUser);
            objApplicationDBContext.SaveChanges();

            return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.OK) { User = new UserApiWrapper(objUser) };
        }

        [HttpPost]
        [ActionName("edit")]
        public UserReturnEntityWrapper Edit(UserApiWrapper objUserApiWrapper)
        {
            if (("" + objUserApiWrapper.Username).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_USERNAME);
            }

            if (("" + objUserApiWrapper.Password).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_PASSWORD);
            }

            if (("" + objUserApiWrapper.FirstName).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_FIRST_NAME);
            }

            if (("" + objUserApiWrapper.LastName).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_LAST_NAME);
            }

            if (("" + objUserApiWrapper.Email).Trim() == "")
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_EMAIL);
            }

            ApplicationDBContext objApplicationDBContext = new ApplicationDBContext();

            Models.User objUser = objApplicationDBContext.Users.Where(x => x.UserID == objUserApiWrapper.UserID).FirstOrDefault();

            if (objUser != null)
            {
                objUserApiWrapper.FillModel(ref objUser);
                objApplicationDBContext.SaveChanges();
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.OK);
            }
            else
            {
                return new UserReturnEntityWrapper(UserReturnEntityWrapper.EnumReturnCodes.WRONG_USER_ID);
            }
        }
    }
}