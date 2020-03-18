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

    public abstract class MainController : PosUserControl<Main, MainViewModel>
    {
        
    }

    public partial class Main : MainController
    {
        public override string HeaderText
        {
            get
            {
                return string.Empty;
            }
        }


        public Main()
        {
            InitializeComponent();
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Main;
            }
        }

        public override bool ShowHomeBtn
        {
            get
            {
                return false;
            }
        }

        public override void Refresh()
        {
            this.CurrentModel.Refresh();
        }

        private void tileAdministration_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Administration);
        }

        private void tileCustomers_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Customers);
        }

        private void tileReserves_Click(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Reserves, new ReservesViewModel());
        }

        private void TileCurrentOrders_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentOrders.Current.CurrentModel.OrderType = OrderBaseTypes.Current;
            CurrentOrders.Current.SetHeaderText("მიმდინარე ორდერები");
            NavigateToControl(UserControlTypes.CurrentOrders);
        }

        private void TileReleasedOrders_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentOrders.Current.CurrentModel.OrderType = OrderBaseTypes.Released;
            CurrentOrders.Current.SetHeaderText("შეკვეთილი ორდერები");
            NavigateToControl(UserControlTypes.CurrentOrders);
        }

        private void TilePostedOrders_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentOrders.Current.CurrentModel.OrderType = OrderBaseTypes.Posted;
            CurrentOrders.Current.SetHeaderText("გადაგზავნილი ორდერები");
            NavigateToControl(UserControlTypes.CurrentOrders);
        }

        private void TileMessages_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Messages);
        }

        private async void TileRetailer_OnClick(object sender, RoutedEventArgs e)
        {
            var customer = DaoController.Current.GetCustomer(App.Current.PosSetting.Settings_GenCustomerCode);
            if (customer == null)
                App.Current.ShowErrorDialog("საცალო კლიენტი ვერ მოიძებნა ბაზაში!", "");
            else
            {

                var order = new SalesHeader
                {
                    No_ = DaoController.Current.GenerateNewKey(App.Current.PosSetting.Settings_SalesHeaderNumberCount),
                    DocumentType = 1,
                    PostingDate = DateTime.Now,
                    Sell_toCustomerNo = customer.No_,
                    Sell_toCustomerName = customer.Name,
                    CustomerPriceGroup = customer.CustomerPriceGroup,
                    SalespersonCode = App.Current.PosSetting.Settings_SalesPersonCode,
                    SalesLines = new List<SalesLine>(),
                    JournalLines = new List<GenJournalLine>(),
                    PaymentSchedules = new List<DataLayer.PaymentSchedule>(),
                    CurrentCustomer = customer
                };
                // Order.Current.SetHeaders("ახალი შეკვეთა", new List<string>() { "დღის განრიგი" });
                Order.Current.PrevUserControl = UserControlTypes.Customers;
                NavigateToControl(UserControlTypes.Order, new OrderViewModel(order, true));
            }
        }
    }
}
