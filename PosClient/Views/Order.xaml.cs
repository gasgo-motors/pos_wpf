﻿using System;
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
using DataLayer;
using DataLayer.Models;
using MahApps.Metro.Controls.Dialogs;
using PosClient.ViewModels;
using PosClient.Helpers;

namespace PosClient.Views
{
    public abstract class OrderController : PosUserControl<Order, OrderViewModel>
    {

    }
    public partial class Order : OrderController
    {
        public Order()
        {
            InitializeComponent();

        }




        public UserControlTypes PrevUserControl { get; set; }
        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Order;
            }
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Disabled;
            }
        }

        public override void Refresh()
        {
            CurrentModel.Refresh();
            Keyboard.Focus(tbx_bar_code);
            tbx_bar_code.Text = "";
            tbx_bar_code.Focus();
            //FocusManager.SetFocusedElement(this, tbx_bar_code);
            
        }

        public override bool ShowHomeBtn
        {
            get { return true; }
        }

        private string _headerText;
        private List<string> _prevHeaders; 

        public void SetHeaders(string text, List<string> path)
        {
            _headerText = text;
            _prevHeaders = path;
        }
        public override string HeaderText
        {
            get { return _headerText; }
        }

        public override List<string> PrevHeaders
        {
            get { return _prevHeaders; }
        }

        private void Btn_products_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Reserves, new ReservesViewModel(CurrentModel));
        }

        private void Btn_payment_schedule_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.PaymentSchedule, new PaymentScheduleViewModel(CurrentModel));
        }

        private void Btn_payment_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Payment, new PaymentViewModel(CurrentModel));
        }

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ შენახვა?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                try
                {
                    CurrentModel.CreateOrder();
                    //if( CurrentModel.IsNew)
                    //    await App.Current.CurrentMainWindow.ShowMessageAsync("ახალი ორდერი შეიქმნა წარმატებით!", "");
                    if (CurrentModel.Order.Sell_toCustomerNo == App.Current.PosSetting.Settings_GenCustomerCode)
                        OpenNewOrder();
                    else
                        NavigateToControl(PrevUserControl);
                }
                catch (Exception ex)
                {
                    App.Current.ShowErrorDialog("შეცდომა შენახვისას!", ex.Message);
                }
            }
        }

        private async void OpenNewOrder()
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

        private async void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ ორდერის გაუქმება?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                DaoController.Current.RemoveSalesHeader(CurrentModel.Order);
                NavigateToControl(PrevUserControl);
            }


        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.IsEditable = true;
            CurrentModel.Refresh();
            CurrentModel.LoadOrderFromNav();
        }

        private void BtnGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(PrevUserControl);
        }

        private void Del_comment_OnClick(object sender, RoutedEventArgs e)
        {
            int line_no = ((sender as FrameworkElement).DataContext as SalesLineShortEntry).LineNo;
            CurrentModel.DeleteSalesLine(line_no);
            CurrentModel.Refresh();
        }

        private async void BtnQuete_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ ორდერის ქვოტირება?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                CurrentModel.QueteOrders();
                NavigateToControl(PrevUserControl);
            }
        }

        private void Tbx_bar_code_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(tbx_bar_code.Text))
                {
                    CurrentModel.AddItemByBarCode(tbx_bar_code.Text);
                    tbx_bar_code.Text = "";
                }
            }
        }

        private void OrderController_Loaded(object sender, RoutedEventArgs e)
        {
            tbx_bar_code.Focus();
            Keyboard.Focus(tbx_bar_code);
        }

        private async void BtnSaveAndSend_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ შენახვა და გადაგზავნა?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                try
                {
                    CurrentModel.CreateOrder();
                }
                catch (Exception ex)
                {
                    App.Current.ShowErrorDialog("შეცდომა შენახვისას!", ex.Message);
                    return;
                }



                var mySettings = new MetroDialogSettings()
                {
                    AnimateShow = false,
                    AnimateHide = false
                };
                var controller = await App.Current.CurrentMainWindow.ShowProgressAsync("მიმდინარეობს ორდერის გადაგზავნა...", "გთხოვთ მოიცადოთ!", settings: mySettings);
                controller.SetIndeterminate();
                string errorNo = "", errorMessage = "";
                int sCount = 0;
                List<RemainingItemEntry> rList = new List<RemainingItemEntry>();

                var orderNo = CurrentModel.Order.No_;
                await Task.Factory.StartNew(() =>
                {
                    sCount =
                        SendServiceManager.Current.SendOrders(
                            new List<string>() { orderNo }, true,
                            out errorNo, out errorMessage, out rList);
                });

                await controller.CloseAsync();

                //var model = new SendToNavResultViewModel(sCount, errorNo, errorMessage, rList);
                //var dialog = (BaseMetroDialog)this.Resources["SendDetail"];
                //dialog.DataContext = model;
                //await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);

                // CurrentModel.Refresh();


                if (CurrentModel.Order.Sell_toCustomerNo == App.Current.PosSetting.Settings_GenCustomerCode)
                    OpenNewOrder();
                else
                    NavigateToControl(PrevUserControl);

            }
        }
    }
}