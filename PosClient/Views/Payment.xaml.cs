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
using PosClient.Helpers;
using PosClient.ViewModels;
using MahApps.Metro.Controls.Dialogs;

namespace PosClient.Views
{
    public abstract class PaymentController : PosUserControl<Payment, PaymentViewModel>
    {

    }
    public partial class Payment : PaymentController
    {
        public Payment()
        {
            InitializeComponent();
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Payment;
            }
        }

        public override void Refresh()
        {
            CurrentModel.Refresh();
        }

        public override bool ShowHomeBtn
        {
            get { return false; }
        }

        public override string HeaderText
        {
            get { return "გადახდა"; }
        }

        public override List<string> PrevHeaders
        {
            get { return new List<string> { "დღის განრიგი", "ახალი შეკვეთა" }; }
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.UpdateModel();
            NavigateToControl(UserControlTypes.Order);
        }


        private async void BtnAddNewRow_OnClick(object sender, RoutedEventArgs e)
        {
            var dt =  CurrentModel.GetNewRow();
            var dialog = (BaseMetroDialog)this.Resources["PayementDetail"];
            dialog.Title = "გადახდის დამატება";
            dialog.DataContext = new PaymentDetailViewModel(dt, CurrentModel.ClientBalance);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }




        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.ParentModel.IsEditable = true;
            PaymentsGrid.Items.Refresh();
        }

        private void BtnGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Order);
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CurrentModel.UpdateAmountPayed();
        }

        private async void Btn_detail_OnClick(object sender, RoutedEventArgs e)
        {
            var dt = (sender as Button).DataContext as GenJournalLine;
            var dialog = (BaseMetroDialog)this.Resources["PayementDetail"];
            dialog.Title = "დეტალური ინფორმაცია გადახდაზე";
            dialog.DataContext = new PaymentDetailViewModel(dt, CurrentModel.ClientBalance);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }

        private void Del_comment_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.DeleteRow((sender as FrameworkElement).DataContext as IGenJournalLine);
            PaymentsGrid.Items.Refresh();
            CurrentModel.UpdateAmountPayed();
        }
    }
}
