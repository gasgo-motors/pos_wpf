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
using DataLayer.Models;
using PosClient.Helpers;
using PosClient.ViewModels;

namespace PosClient.Views
{
    public abstract class PaymentScheduleController : PosUserControl<PaymentSchedule, PaymentScheduleViewModel>
    {

    }


    public partial class PaymentSchedule : PaymentScheduleController
    {
        public PaymentSchedule()
        {
            InitializeComponent();
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.PaymentSchedule;
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
            get { return "გადახდების გრაფიკი"; }
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

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Order);
        }

        private void BtnAddNewRow_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.AddNewRow();
            SchedulesGrid.Items.Refresh();
        }

        private void BtnRmoveRow_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.DeleteCurrentRow();
            SchedulesGrid.Items.Refresh();
            CurrentModel.UpdateAmountPayed();
        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.ParentModel.IsEditable = true;
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            CurrentModel.UpdateAmountPayed();
        }

        private void Del_comment_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.DeleteRow((sender as FrameworkElement).DataContext as IPaymentSchedule);
            SchedulesGrid.Items.Refresh();
            CurrentModel.UpdateAmountPayed();
        }
    }
}
