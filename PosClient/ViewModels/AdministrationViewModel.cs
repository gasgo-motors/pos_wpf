using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using BusinessLayer;

namespace PosClient.ViewModels
{
    public class AdministrationViewModel : PosViewModel
    {
        public bool IsManager
        {
            get { return App.Current.User.UserType == PosUserTypes.Manager; }
        }

        public Visibility AdministrationVisible
        {
            get { return IsManager ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DistributorVisibility
        {
            get { return App.Current.User.UserType == PosUserTypes.Distributor ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void Refresh()
        {
            RaisePropertyChanged(() => IsManager);
            RaisePropertyChanged(() => AdministrationVisible);
            RaisePropertyChanged(() => DistributorVisibility);
            
        }
    }
}
