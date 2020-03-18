using MahApps.Metro.Controls.Dialogs;
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
using PosClient.ViewModels;

namespace PosClient.Views.Dialogs
{

    public partial class PosUserDetail : UserControl
    {
       
        public PosUserDetail()
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
                var res = (this.DataContext as PosUserDetailViewModel).SaveUser();
                if (!string.IsNullOrEmpty(res))
                    lbl_error.Content = res;
                else
                {
                    await App.Current.CurrentMainWindow.ShowMessageAsync("ოპერაცია დასრულდა წარმატებით", "");
                    await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
                    PosUsers.Current.Refresh();
                }
            }

        }

        private async void Btn_cancel_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
