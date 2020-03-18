using PosClient.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PosClient.Helpers
{
    public interface IPosUserControl
    {
        //KM
        UserControlTypes UserControlType { get; }

        void Refresh();

        bool ShowHomeBtn { get; }

        int HomeButtonSize { get; }

        int HomeIconSize { get; }

        string HeaderText { get; }
        
        List<string> PrevHeaders { get; }
        ScrollBarVisibility ScrollBarVis { get;}

    }


    public abstract class PosUserControl<U, M> : UserControl, IPosUserControl where U : new() where M : PosViewModel
    {


        private static readonly Lazy<U> lazy =
            new Lazy<U>(() => new U());


        public static U Current { get { return lazy.Value; } }

        public M CurrentModel
        {
            get
            {
                return DataContext as M;
            }
        }

        public abstract UserControlTypes UserControlType { get; }

        public abstract bool ShowHomeBtn
        {
            get;
        }

        public virtual int HomeButtonSize
        {
            get { return 48; }
        }

        public virtual int HomeIconSize
        {
            get { return 20; }
        }

        public  abstract string HeaderText
        {
            get;
        }

        public virtual List<string> PrevHeaders
        {
            get
            {
                return new List<string>();
            }
        }

        public virtual ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Auto;
            }
        }

        public abstract void Refresh();

        public void NavigateToControl(UserControlTypes userControlType, PosViewModel datacontext = null)
        {
            var dict = new List<IPosUserControl>() { Login.Current, Main.Current, Administration.Current, Settings.Current, Customers.Current, Reserves.Current, PosUsers.Current, Order.Current, PaymentSchedule.Current, Payment.Current, CurrentOrders.Current, Messages.Current, CurrentGenJournals.Current, PaymentSchedules.Current, CurrentQuotes.Current };
            var uc = dict.First(i => i.UserControlType == userControlType) as UserControl;
            if (datacontext != null)
                uc.DataContext = datacontext;
            ((IPosUserControl)uc).Refresh();
            App.Current.CurrentMainWindow.SetUserControl(uc);
        }
    }

    public enum UserControlTypes
    {
        Login,
        Main,
        Administration,
        Settings,
        Customers,
        Reserves,
        PosUsers,
        Order,
        PaymentSchedule,
        Payment,
        CurrentOrders,
        Messages,
        CurrentGenJournals,
        PaymentSchedules,
        ReleasedQuotes
    }
}
