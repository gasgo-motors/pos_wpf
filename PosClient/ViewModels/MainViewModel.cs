using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BusinessLayer;

namespace PosClient.ViewModels
{
    public class MainViewModel : PosViewModel
    {

        public Visibility ShopVisibility
        {
            get { return App.Current.User.UserType == PosUserTypes.Shop ? Visibility.Visible : Visibility.Collapsed; }
        }


        public Visibility NonDistributorVisibility
        {
            get { return App.Current.User.UserType != PosUserTypes.Distributor ? Visibility.Visible : Visibility.Collapsed; }
        }


        public void Refresh()
        {
            RaisePropertyChanged(() => ShopVisibility);
            RaisePropertyChanged(() => NonDistributorVisibility);
        }
    }
}
