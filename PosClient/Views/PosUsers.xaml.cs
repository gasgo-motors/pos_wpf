using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLayer;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PosClient.Helpers;
using PosClient.ViewModels;
using PosClient.Views.Dialogs;

namespace PosClient.Views
{
    public abstract class PosUsersController : PosUserControl<PosUsers, PosUsersViewModel>
    {

    }


    public partial class PosUsers : PosUsersController
    {
        public PosUsers()
        {
            InitializeComponent();
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.PosUsers;
            }
        }
        public override void Refresh()
        {
            CurrentModel.Refresh();
        }

        public override bool ShowHomeBtn
        {
            get { return true; }
        }
        public override string HeaderText
        {
            get
            {
                return "მომხმარებლების მართვა";
            }
        }

        private async void ButtonDetail_OnClick(object sender, RoutedEventArgs e)
        {
            var dt = (sender as Button).DataContext as PosUser;
            var dialog = (BaseMetroDialog)this.Resources["UserDetail"];
            dialog.Title = "დეტალური ინფორმაცია მომხმარებელზე";
            var user = PosUsersManager.Current.GetUser(dt.UserName);
            dialog.DataContext = new PosUserDetailViewModel(user);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["UserDetail"];
            dialog.Title = "ახალი მომხმარებლის დამატება";
            var user = new PosUser();
            dialog.DataContext = new PosUserDetailViewModel(user);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }
    }
}
