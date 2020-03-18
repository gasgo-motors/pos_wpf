using BusinessLayer;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PosClient.Helpers;
using PosClient.ViewModels;
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
using DataLayer;
using DataLayer.Models;

namespace PosClient.Views
{
    public abstract class AdministrationController : PosUserControl<Administration, AdministrationViewModel>
    {

    }


    public partial class Administration : AdministrationController
    {
        public Administration()
        {
            InitializeComponent();
        }

        public override string HeaderText
        {
            get
            {
                return "ადმინისტრირება";
            }
        }

        public override void Refresh()
        {
            CurrentModel.Refresh();
        }

        public override bool ShowHomeBtn
        {
            get
            {
                return true;
            }
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Administration;
            }
        }

        private void tileParameters_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Settings);
        }

        private async void BtnSync_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }

        private async void btnChooseSyncType_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];
            var chkReserves = dialog.FindChild<CheckBox>("chkReserves");
            var chkProducts = dialog.FindChild<CheckBox>("chkProducts");
            var chkCustomers = dialog.FindChild<CheckBox>("chkCustomers");
            var chkGeneral = dialog.FindChild<CheckBox>("chkGeneral");
            var chkReservesShort = dialog.FindChild<CheckBox>("chkReservesShort");
            //var chkPictures = dialog.FindChild<CheckBox>("chkPictures");
            var chkSalesPrices = dialog.FindChild<CheckBox>("chkSalesPrices");
            bool withPictures =  true;
            var st = new HashSet<SynchTypes>();
            if (chkGeneral.IsChecked == true) st.Add(SynchTypes.General);
            if (chkReserves.IsChecked == true) st.Add(SynchTypes.Reserves);
            if (chkProducts.IsChecked == true) st.Add(SynchTypes.Products);
            if (chkCustomers.IsChecked == true) st.Add(SynchTypes.Customers);
            if(chkReservesShort.IsChecked == true) st.Add(SynchTypes.ReservesShort);
            if (chkSalesPrices.IsChecked == true) st.Add(SynchTypes.SalesPrices);
            await App.Current.CurrentMainWindow.HideMetroDialogAsync(dialog);
            var mySettings = new MetroDialogSettings()
            {
                AnimateShow = false,
                AnimateHide = false
            };
            if (SynchronizationManager.Current.HasUnsendObjects())
            {
                App.Current.ShowErrorDialog("შეცდომა სინრონიზაციისას", "არსებობს გადაუგზავნელი ობეიქტები! გთხოვთ გადააგზავნოთ!");
            }
            else
            {
                var controller =
                    await
                        App.Current.CurrentMainWindow.ShowProgressAsync("მიმდინარეობს სინქრონიზაცია...",
                            "გთხოვთ მოიცადოთ!", settings: mySettings);
                controller.SetIndeterminate();
                await Task.Factory.StartNew(() =>
                {
                    SynchronizationManager.Current.Synchronize(st, withPictures,
                        (title, message, percent) =>
                        {
                            controller.SetProgress(percent/100.0);
                            controller.SetMessage(message);
                            if (!string.IsNullOrEmpty(title))
                                controller.SetTitle(title);
                        }, App.Current.User.UserType
                        );
                });
                await controller.CloseAsync();

                

                await App.Current.CurrentMainWindow.ShowMessageAsync("სინქრონიზაცია დასრულდა წარმატებით", "");

                

            }
        }

        private async void btnCancelSycnType_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];
            await App.Current.CurrentMainWindow.HideMetroDialogAsync(dialog);
        }

        private void tileUsersManagement_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.PosUsers);
        }

        private async void BtnSendOrders_Click(object sender, RoutedEventArgs e)
        {
            if (await App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ გადაგზავნა?", "", MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                var ordersList = DaoController.Current.GetOrdersList(OrderBaseTypes.Current).Select(i => i.No_).ToList();



                    var mySettings = new MetroDialogSettings()
                    {
                        AnimateShow = false,
                        AnimateHide = false
                    };
                    var controller = await App.Current.CurrentMainWindow.ShowProgressAsync("მიმდინარეობს ინფორმაციის გადაგზავნა...", "გთხოვთ მოიცადოთ!", settings: mySettings);
                    controller.SetIndeterminate();
                    string errorNo = "", errorMessage = "";
                    int sCount = 0;
                    List<RemainingItemEntry> rList = new List<RemainingItemEntry>();

                    await Task.Factory.StartNew(() =>
                    {


                        sCount =
                            SendServiceManager.Current.SendOrders(
                                ordersList, true,
                                out errorNo, out errorMessage, out rList);

                        SynchronizationManager.Current.SynchronizeMessages();

                    });

                    await controller.CloseAsync();

                    var model = new SendToNavResultViewModel(sCount, errorNo, errorMessage, rList);
                    var dialog = (BaseMetroDialog)App.Current.CurrentMainWindow.Resources["SendDetailAll"];
                    dialog.DataContext = model;
                    await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);

            }
        }
    }
}
