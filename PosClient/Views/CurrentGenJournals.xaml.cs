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
using DataLayer.Extensions;
using PosClient.Helpers;
using PosClient.ViewModels;

namespace PosClient.Views
{

    public abstract class CurrentGenJournalsController : PosUserControl<CurrentGenJournals, CurrentGenJournalsViewModel>
    {

    }

    public partial class CurrentGenJournals : CurrentGenJournalsController
    {
        public CurrentGenJournals()
        {
            InitializeComponent();
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Disabled;
            }
        }

        public override UserControlTypes UserControlType
        {
            get { return UserControlTypes.CurrentGenJournals; }
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
            get { return "მიმდინარე გადახდები"; }
        }

        private void Btn_edit_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.InEditMode = true;
            CurrentModel.UpdateEditiVisibilities();
        }

        private void Btn_save_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.SaveDates();
        }

        private void Btn_cancel_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.Refresh();
        }

        private void Del_payment_OnClick(object sender, RoutedEventArgs e)
        {
            var gn = ((sender as FrameworkElement).DataContext as GenJouranlView);
            CurrentModel.DeleteRow(gn);

        }

    }
}
