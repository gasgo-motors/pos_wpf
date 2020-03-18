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
using PosClient.Helpers;

namespace PosClient.Views
{

    public abstract class PaymentSchedulesController : PosUserControl<PaymentSchedules, PaymentSchedulesViewModel>
    {

    }

    public partial class PaymentSchedules : PaymentSchedulesController
    {
        public PaymentSchedules()
        {
            InitializeComponent();
        }

        public override UserControlTypes UserControlType
        {
            get { return UserControlTypes.PaymentSchedules; }
        }

        public override void Refresh()
        {
            
        }

        public override bool ShowHomeBtn
        {
            get { return true; }
        }

        public override string HeaderText
        {
            get { return "გადახდის გრაფიკი"; }
        }
    }
}
