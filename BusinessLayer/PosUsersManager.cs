using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class PosUsersManager : PosManager<PosUsersManager>
    {
        public  PosUser CurrentUser { get; set; }

        public  bool Authenticate(string username, string password, out string message)
        {
            var dbUser = DaoController.Current.getUserByUserNameAndPassword(username, password);
            if(dbUser == null)
            {
                message = "არასწორი მომხმარებელი ან პაროლი!";
                return false;
            }
            var user = new PosUser
            {
                FirstName = dbUser.POSUser_FirstName,
                IsGood = dbUser.POSUser_IsBlocked == false,
                LastName = dbUser.POSUser_LastName,
                ModifiedDate = dbUser.ModifiedDate,
                Password = dbUser.POSUser_Password,
                SalesPersonCode = dbUser.POSUser_SalesPerson_Code,
                UserName = dbUser.POSUser_UserID,
                UserType = (PosUserTypes)(dbUser.POSUser_type.HasValue ? dbUser.POSUser_type.Value : 1)
            };
            if(!user.IsGood)
            {
                message = "მომხმარებელი დაბლოკილია!";
                return false;
            }
            CurrentUser = user;
            message = string.Empty;
            return true;
        }

        public List<PosUser> GetUsers()
        {
            return DaoController.Current.getUsers().Select(dbUser => new PosUser
            {
                FirstName = dbUser.POSUser_FirstName,
                IsGood = dbUser.POSUser_IsBlocked == false,
                LastName = dbUser.POSUser_LastName,
                ModifiedDate = dbUser.ModifiedDate,
                Password = dbUser.POSUser_Password,
                SalesPersonCode = dbUser.POSUser_SalesPerson_Code,
                UserName = dbUser.POSUser_UserID,
                UserType = (PosUserTypes) (dbUser.POSUser_type.HasValue ? dbUser.POSUser_type.Value : 1)
            }).ToList();
        }

        public PosUser GetUser(string username)
        {
            var dbUser = DaoController.Current.getUser(username);
            var user = new PosUser
            {
                FirstName = dbUser.POSUser_FirstName,
                IsGood = dbUser.POSUser_IsBlocked == false,
                LastName = dbUser.POSUser_LastName,
                ModifiedDate = dbUser.ModifiedDate,
                Password = dbUser.POSUser_Password,
                SalesPersonCode = dbUser.POSUser_SalesPerson_Code,
                UserName = dbUser.POSUser_UserID,
                UserType = (PosUserTypes)(dbUser.POSUser_type.HasValue ? dbUser.POSUser_type.Value : 1)
            };
            return user;
        }

        public void SaveUser(PosUser user, string olduserName)
        {
            DaoController.Current.SaveUser(olduserName, user.UserName,user.Password,user.FirstName,user.LastName,!user.IsGood, user.UserTypeId,user.SalesPersonCode );
        }

        public List<Salesperson_Purchaser> GetSalesPersons()
        {
            return DaoController.Current.GetSalesPersons();
        }

        public List<PosUserType> GetUserTypes()
        {
            return DaoController.Current.GetUserTypes();
        }

    }
}
