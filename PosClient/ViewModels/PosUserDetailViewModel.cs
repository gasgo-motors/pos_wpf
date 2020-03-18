using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer;
using DataLayer;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    class PosUserDetailViewModel : PosViewModel
    {
        public PosUser CurrentPosUser { get; set; }

        private string oldUsername;

        public bool IsEnabled
        {
            get { return App.Current.User.UserType == PosUserTypes.Manager; }
        }

        public PosUserDetailViewModel() { }
        public PosUserDetailViewModel(PosUser user)
        {
            CurrentPosUser = user;
            oldUsername = user.UserName;
            Refresh();
        }
        public void Refresh()
        {
            salesPersons = PosUsersManager.Current.GetSalesPersons();
            UserTypes = PosUsersManager.Current.GetUserTypes();
        }

        private List<Salesperson_Purchaser> salesPersons;
        public List<Salesperson_Purchaser> SalesPersons
        {
            get { return salesPersons; }
            set
            {
                if (salesPersons != value)
                {
                    salesPersons = value;
                    RaisePropertyChanged(() => SalesPersons);
                }
            }
        }


        private List<PosUserType> userTypes;
        public List<PosUserType> UserTypes
        {
            get { return userTypes; }
            set
            {
                if (userTypes != value)
                {
                    userTypes = value;
                    RaisePropertyChanged(() => UserTypes);
                }
            }
        }

        public string SaveUser()
        {
            if (String.IsNullOrEmpty(CurrentPosUser.UserName))
            {
                return "შეიყვანეთ მომხმარებლის username";
            }
            if (String.IsNullOrEmpty(CurrentPosUser.Password))
            {
                return "შეიყვანეთ მომხმარებლის პაროლი";
            }
            if (String.IsNullOrEmpty(CurrentPosUser.FirstName))
            {
                return "შეიყვანეთ მომხმარებლის სახელი";
            }
            if (String.IsNullOrEmpty(CurrentPosUser.LastName))
            {
                return "შეიყვანეთ მომხმარებლის გვარი";
            }
            try
            {
                PosUsersManager.Current.SaveUser(CurrentPosUser, oldUsername);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return null;
        }

    }
}
