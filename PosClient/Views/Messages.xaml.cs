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
using PosClient.Helpers;
using PosClient.ViewModels;

namespace PosClient.Views
{
    public abstract class MessagesController : PosUserControl<Messages, MessagesViewModel>
    {

    }

    public partial class Messages
    {
        public Messages()
        {
            InitializeComponent();
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get { return ScrollBarVisibility.Disabled; }
        }

        public override UserControlTypes UserControlType
        {
            get { return UserControlTypes.Messages; }
        }
        public override void Refresh()
        {
            CurrentModel.Refresh();
        }

        public override bool ShowHomeBtn
        {
            get { return true; }
        }

        public override string HeaderText
        {
            get { return "შეტყობინებები"; }
        }

        private void Grid_user_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentModel.SelectUser((sender as FrameworkElement).DataContext as MessageUser);
        }

        private void Btn_del_message_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.DeleteMessage((sender as FrameworkElement).DataContext as POSMessageEntry);
        }

        private void Btn_send_new_message_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.SendNewMessage();
        }
    }
}
