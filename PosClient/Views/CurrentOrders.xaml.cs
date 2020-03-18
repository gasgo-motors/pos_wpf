using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using DataLayer;
using DataLayer.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PosClient.Helpers;
using PosClient.ViewModels;

namespace PosClient.Views
{
    public abstract class CurrentOrdersController : PosUserControl<CurrentOrders, CurrentOrdersViewModel>
    {

    }

    public partial class CurrentOrders : CurrentOrdersController
    {
        public CurrentOrders()
        {
            InitializeComponent();
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.CurrentOrders;
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

        public override bool ShowHomeBtn
        {
            get { return true; }
        }

        private string _headerText;

        public void SetHeaderText(string text)
        {
            _headerText = text;

        }


        public override string HeaderText
        {
            get { return _headerText; }
        }

        private void Btn_order_no_OnClick(object sender, RoutedEventArgs e)
        {
            string orderNo = ((sender as Button).DataContext as OrderShortEntry).No_;
            var order = DaoController.Current.GetCurrentOrder(orderNo, CurrentModel.OrderType, true);
            var curCustomer = DaoController.Current.GetCustomer(order.Sell_toCustomerNo);
            order.CurrentCustomer = curCustomer;
            Order.Current.SetHeaders("შეკვეთა: " + orderNo  , new List<string>() { HeaderText });
            Order.Current.PrevUserControl = UserControlTypes.CurrentOrders;
            NavigateToControl(UserControlTypes.Order, new OrderViewModel(order));
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            var ck = sender as ToggleSwitch;
            CurrentModel.CheckAll = ck.IsChecked == true;
            
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            var ck = sender as ToggleSwitch;
            CurrentModel.CheckAll = ck.IsChecked == true;
        }

        private async void btn_send_orders_Click(object sender, RoutedEventArgs e)
        {
            var ordersList = CurrentModel.OrdersList.Where(i => i.IsChecked).Select(i => i.No_).ToList();
            if (ordersList.Count == 0)
                await App.Current.CurrentMainWindow.ShowMessageAsync("მონიშნეთ ორდერი!", "");
            else
            {
                if (await App.Current.ShowConfirmMessage("გსურთ მონიშნული ორდერების გადაგზავნა?", ""))
                {

                    var mySettings = new MetroDialogSettings()
                    {
                        AnimateShow = false,
                        AnimateHide = false
                    };
                    var controller = await App.Current.CurrentMainWindow.ShowProgressAsync("მიმდინარეობს ორდერების გადაგზავნა...", "გთხოვთ მოიცადოთ!", settings: mySettings);
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
                    });

                    await controller.CloseAsync();

                    var model = new SendToNavResultViewModel(sCount, errorNo, errorMessage, rList);
                    var dialog = (BaseMetroDialog)this.Resources["SendDetail"];
                    dialog.DataContext = model;
                    await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);

                    CurrentModel.Refresh();
                }
            }
        }

        private void ToggleSwitch_OnChecked(object sender, RoutedEventArgs e)
        {
            var ck = sender as ToggleSwitch;
            (ck.DataContext as OrderShortEntry).IsChecked = ck.IsChecked == true;


        }

        private void ToggleSwitch_OnUnchecked(object sender, RoutedEventArgs e)
        {
            var ck = sender as ToggleSwitch;
            (ck.DataContext as OrderShortEntry).IsChecked = ck.IsChecked == true;
        }

        private void Btn_search_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.Filter();
        }

        private void Btn_clear_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.ClearFilter();
        }

        private void Btn_synchronize_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SynchronizationManager.Current.SynchronizeReleasedOrders();
                App.Current.CurrentMainWindow.ShowMessageAsync("სინქრონიზაცია დასრულდა წარმატებით", "");
                CurrentModel.Refresh();
            }
            catch (Exception ex)
            {
                App.Current.ShowErrorDialog("შეცდომა სინქრონიზაციისას", ex.Message);
            }
        }
    }
}
