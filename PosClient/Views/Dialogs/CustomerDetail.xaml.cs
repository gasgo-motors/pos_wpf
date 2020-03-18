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
using MahApps.Metro.Controls.Dialogs;
using PosClient.ViewModels;

namespace PosClient.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for CustomerDetail.xaml
    /// </summary>
    public partial class CustomerDetail : UserControl
    {
        public CustomerDetail()
        {
            InitializeComponent();
        }

        private async void Btn_save_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ ცვლილების შენახვა?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                var res = (this.DataContext as CustomerDetailViewModel).SaveCustomer();
                if (string.IsNullOrEmpty(res))
                {
                    //  await App.Current.CurrentMainWindow.ShowMessageAsync("ოპერაცია დასრულდა წარმატებით", "");
                    await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
                    Customers.Current.Refresh();
                }
            }
        }

        private async void Btn_cancel_OnClick(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as CustomerDetailViewModel).IsNew)
                await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
            else
            {
                if (await App.Current.ShowConfirmMessage("გსურთ კლიენტის გაუქმება?", ""))
                {
                    if ((this.DataContext as CustomerDetailViewModel).HasOrders)
                    {
                        App.Current.ShowErrorDialog("შეცდომა გაუქმებისას", "კლიენტს აქვს მიბმული ორდერები!");
                    }
                    else
                    {
                        try
                        {
                            (this.DataContext as CustomerDetailViewModel).RemoveCusetomer();
                        }
                        catch (Exception ex)
                        {
                            App.Current.ShowErrorDialog("შეცდომა გაუქმებისას", ex.Message);
                        }
                        await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
                        Customers.Current.Refresh();
                    }
                }
            }
        }

        private async void Btn_close_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
        }
    }
}

