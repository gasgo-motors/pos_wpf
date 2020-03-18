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

namespace PosClient.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for SendtoNavResult.xaml
    /// </summary>
    public partial class SendtoNavResult : UserControl
    {
        public SendtoNavResult()
        {
            InitializeComponent();
        }

        private async void Btn_exit_from_result_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
        }
    }
}
