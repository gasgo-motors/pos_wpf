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
using PosClient.ge.mof.revenue.www;
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if( Tbx_vat.Text.Length == 9 || Tbx_vat.Text.Length == 11)
            {
                NtosService client = new NtosService();
                string name;
                if(user_id <= 0)
                {
                    client.get_ser_users(App.Current.PosSetting.Settings_RsUsername , App.Current.PosSetting.Settings_RsPassword, out user_id);
                }
                if (user_id > 0)
                {
                    var un_id = client.get_un_id_from_tin(user_id, Tbx_vat.Text, App.Current.PosSetting.Settings_RsServiceUsername , App.Current.PosSetting.Settings_RsPassword, out name);
                    if (!string.IsNullOrEmpty(name))
                    {
                        (this.DataContext as CustomerDetailViewModel).SetName(name);
                        Tbx_customer_name.Text = name;
                    }
                }
            }
        }

        private int user_id;
    }
}

