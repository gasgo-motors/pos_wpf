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
using PosClient.Helpers;
using PosClient.ViewModels;

namespace PosClient.Views
{

    public abstract class CurrentQuotesController : PosUserControl<CurrentQuotes, CurrentQuotesViewModel>
    {

    }


    public partial class CurrentQuotes : CurrentQuotesController
    {
        public CurrentQuotes()
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
            get { return UserControlTypes.ReleasedQuotes; }
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
            get { return "მიმდინარე კვოტები" ; }
        }
    }
}
