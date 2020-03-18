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
    /// Interaction logic for ItemCommentsDetail.xaml
    /// </summary>
    public partial class ItemCommentsDetail : UserControl
    {
        public ItemCommentsDetail()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            int lineNo = ((sender as Button).DataContext as Comment_Line).Line_No_;
            (DataContext as ItemCommentsDetailViewModel).RemoveComment(lineNo);
        }

        private void Btn_add_comment_OnClick(object sender, RoutedEventArgs e)
        {
            (DataContext as ItemCommentsDetailViewModel).AddNewComment();
        }

        private async void Btn_cancel_comment_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync((Parent as BaseMetroDialog));
        }
    }
}
