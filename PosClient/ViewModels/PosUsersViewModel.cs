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
    public class PosUsersViewModel : PosViewModel
    {
        private List<PosUser> users;
        public List<PosUser> Users
        {
            get { return users; }
            set
            {
                if (users != value)
                {
                    users = value;
                    RaisePropertyChanged(() => Users);
                }
            }
        }




        public void Refresh()
        {
            Users = PosUsersManager.Current.GetUsers();
        }



    }
}
