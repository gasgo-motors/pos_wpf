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
using MahApps.Metro.Controls.Dialogs;
using PosClient.ViewModels;

namespace PosClient.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for PaymentDetail.xaml
    /// </summary>
    public partial class PaymentDetail : UserControl
    {

        public Payment ParentView { get; set; }
        public PaymentDetail()
        {
            InitializeComponent();
        }

        private async void Btn_save_record_OnClick(object sender, RoutedEventArgs e)
        {
            var errorText = (this.DataContext as PaymentDetailViewModel).Save();
            if (string.IsNullOrEmpty(errorText))
            {
                await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
                
                Payment.Current.PaymentsGrid.Items.Refresh();
                Payment.Current.CurrentModel.UpdateAmountPayed();
            }

        }

        private async void Btn_go_back_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
        }

        private async void Btn_delete_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
            Payment.Current.CurrentModel.DeleteCurrentRow();
            Payment.Current.PaymentsGrid.Items.Refresh();
            Payment.Current.CurrentModel.UpdateAmountPayed();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
