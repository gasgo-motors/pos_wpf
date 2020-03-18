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
    /// Interaction logic for CutomErrorDialog.xaml
    /// </summary>
    public partial class CutomErrorDialog : UserControl
    {
        public CutomErrorDialog()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
        }
    }
}
