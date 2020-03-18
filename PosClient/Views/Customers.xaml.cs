using DataLayer;
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
using BusinessLayer;

namespace PosClient.Views
{
    public abstract class CustomersController : PosUserControl<Customers, CustomersViewModel>
    {

    }


    public partial class Customers : CustomersController
    {
        public Customers()
        {
            InitializeComponent();
            btnCurrentDay.Focus();
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Customers;
            }
        }

        public override bool ShowHomeBtn
        {
            get { return true; }
        }

        public override string HeaderText
        {
            get
            {
                return string.Format("დღის განრიგი: {0}", DateTime.Now.ToString("dd.MM.yyyy"));
            }
        }

        public override void Refresh()
        {
            CurrentModel.Refresh();
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Disabled;
            }
        }

        private void btnCurrentDay_Click(object sender, RoutedEventArgs e)
        {
            CurrentModel.CustomerButtonType = CustomerButtonTypes.Today;
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            CurrentModel.CustomerButtonType = CustomerButtonTypes.All;
        }

        private async void BtnNew_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomerDetail"];
            dialog.DataContext = new CustomerDetailViewModel();
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }

        private async void BtnNewOrder_OnClick(object sender, RoutedEventArgs e)
        {
            var order = CurrentModel.CreateNewSalesHeader();
            if (order == null)
                await App.Current.CurrentMainWindow.ShowMessageAsync("აირჩიეთ კლიენტი!", "");
            else
            {
                Order.Current.SetHeaders("ახალი შეკვეთა", new List<string>() {"დღის განრიგი"});
                Order.Current.PrevUserControl = UserControlTypes.Customers;
                NavigateToControl(UserControlTypes.Order, new OrderViewModel(order, true));
            }
        }

        private async void Del_comment_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomerDetail"];
            dialog.DataContext = new CustomerDetailViewModel((sender as FrameworkElement).DataContext as Customer);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }

        private async void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(App.Current.User.UserType == PosUserTypes.Distributor) return;
            var order = CurrentModel.CreateNewSalesHeader();
            if (order == null)
                await App.Current.CurrentMainWindow.ShowMessageAsync("აირჩიეთ კლიენტი!", "");
            else
            {
                Order.Current.SetHeaders("ახალი შეკვეთა", new List<string>() { "დღის განრიგი" });
                Order.Current.PrevUserControl = UserControlTypes.Customers;
                NavigateToControl(UserControlTypes.Order, new OrderViewModel(order, true));
            }
        }

        private void BtnCurrentGenJournals_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.CurrentGenJournals);
        }

        private async void BtnPaymentschedules_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.SelectedCustomer == null)
                await App.Current.CurrentMainWindow.ShowMessageAsync("აირჩიეთ კლიენტი!", "");
            else
            {
                NavigateToControl(UserControlTypes.PaymentSchedules, new PaymentSchedulesViewModel(CurrentModel.SelectedCustomer));
            }
        }

        private async void BtnPyment_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.SelectedCustomer == null)
                await App.Current.CurrentMainWindow.ShowMessageAsync("აირჩიეთ კლიენტი!", "");
            else
            {
                
                var dt =
                 new GenJournalLine()
                 {
                     TemplateName = App.Current.PosSetting.Settings_JnlTemplateName,
                     BatchName = App.Current.PosSetting.Settings_JnlBatchName,
                     DocumentNo = DaoController.Current.GenerateNewKey(App.Current.PosSetting.Settings_SalesHeaderNumberCount, App.Current.PosSetting.Settings_JnlGeneralBatchName),
                     DocumentType = 1,
                     PostingDate = DateTime.Now,
                     AccountType =
                        (App.Current.User.UserType == PosUserTypes.Distributor ||
                         App.Current.User.UserType == PosUserTypes.PreSaler)
                            ? 2
                            : 3,
                     AccountNo_ =
                        (App.Current.User.UserType == PosUserTypes.Distributor ||
                         App.Current.User.UserType == PosUserTypes.PreSaler)
                            ? App.Current.PosSetting.Settings_RespEmployee
                            : null,
                     Bal_AccountType = 1,
                     Bal_AccountNo_ = CurrentModel.SelectedCustomer.No_,
                     Salespers__Purch_Code = App.Current.PosSetting.Settings_SalesPersonCode,
                     ResponsibilityCenter = App.Current.PosSetting.Settings_ResponsibilityCenter,
                     PaymentMethodCode = "CASH",
                     IsNew = true,
                     IsGeneral = true
                 };
                var dialog = (BaseMetroDialog)this.Resources["PayementDetail"];
                dialog.Title = "გადახდის დამატება";
                dialog.DataContext = new PaymentDetailViewModel(dt, CurrentModel.SelectedCustomer.Balance);
                await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
            }


        }

        private async void BtnReleasedQuetes_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.SelectedCustomer == null)
                await App.Current.CurrentMainWindow.ShowMessageAsync("აირჩიეთ კლიენტი!", "");
            else
            {
                NavigateToControl(UserControlTypes.ReleasedQuotes, new CurrentQuotesViewModel(CurrentModel.SelectedCustomer));
            }
        }

        private async void BtnShortSync_OnClick(object sender, RoutedEventArgs e)
        {
            var st = new HashSet<SynchTypes>();
            st.Add(SynchTypes.ReservesShort);
            var mySettings = new MetroDialogSettings()
            {
                AnimateShow = false,
                AnimateHide = false
            };
            var controller =
                await
                    App.Current.CurrentMainWindow.ShowProgressAsync("მიმდინარეობს სინქრონიზაცია...",
                        "გთხოვთ მოიცადოთ!", settings: mySettings);

            controller.SetIndeterminate();
            await Task.Factory.StartNew(() =>
            {
                SynchronizationManager.Current.Synchronize(st, false,
                    (title, message, percent) =>
                    {
                        controller.SetProgress(percent / 100.0);
                        controller.SetMessage(message);
                        if (!string.IsNullOrEmpty(title))
                            controller.SetTitle(title);
                    }, App.Current.User.UserType
                    );
            });
            await controller.CloseAsync();
            //await App.Current.CurrentMainWindow.ShowMessageAsync("სინქრონიზაცია დასრულდა წარმატებით", "");
        }
    }
}
