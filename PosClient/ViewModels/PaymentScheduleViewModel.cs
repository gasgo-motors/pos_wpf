using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DataLayer;
using DataLayer.Models;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class PaymentScheduleViewModel : PosViewModel
    {
        public OrderViewModel ParentModel { get; set; }
        public PaymentScheduleViewModel() { }




        public PaymentScheduleViewModel(OrderViewModel parentModel)
        {
            ParentModel = parentModel;
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                _schedules = new List<PaymentSchedule>();
            else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Released)
                _schedules = new List<ReleasedPaymentSchedule>();
            else
                _schedules = new List<PostedPaymentSchedule>();
            foreach (var s in parentModel.PaymentSchedules)
            {
                if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                    (_schedules as List<PaymentSchedule>).Add(CreateNewPaymentSchedule(s.Amount, s.Date) as PaymentSchedule);
                else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Released)
                    (_schedules as List<ReleasedPaymentSchedule>).Add(CreateNewPaymentSchedule(s.Amount, s.Date) as ReleasedPaymentSchedule);
                else
                    (_schedules as List<PostedPaymentSchedule>).Add(CreateNewPaymentSchedule(s.Amount, s.Date) as PostedPaymentSchedule);
            }
        }


        private IEnumerable<IPaymentSchedule> _schedules;
        public IEnumerable<IPaymentSchedule> Schedules
        {
            get { return _schedules; }
            set
            {
                if (value != _schedules)
                {
                    _schedules = value;
                    RaisePropertyChanged(() => Schedules);
                }
            }
        }

        public decimal? AmountPayed
        {
            get
            {
                if (_schedules == null) return 0;
                if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                    return (_schedules as List<PaymentSchedule>).Sum(i => i.Amount);
                else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Released)
                    return (_schedules as List<ReleasedPaymentSchedule>).Sum(i => i.Amount);
                else
                    return (_schedules as List<PostedPaymentSchedule>).Sum(i => i.Amount);
            }
        }

        public void UpdateAmountPayed()
        {
            RaisePropertyChanged(() => AmountPayed);
            RefreshSummary();
        }

        private IPaymentSchedule _selectedSchedule;
        public IPaymentSchedule SelectedSchedule
        {
            get { return _selectedSchedule; }
            set
            {
                if (_selectedSchedule != value)
                {
                    _selectedSchedule = value;
                }
            }
        }

        public void Refresh()
        {
            RefreshSummary();
        }

        public void AddNewRow()
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                (Schedules as List<PaymentSchedule>).Add(CreateNewPaymentSchedule(0, DateTime.Now) as PaymentSchedule);
            else
                (Schedules as List<ReleasedPaymentSchedule>).Add(CreateNewPaymentSchedule(0, DateTime.Now) as ReleasedPaymentSchedule);
        }

        public IPaymentSchedule CreateNewPaymentSchedule(decimal? amount, DateTime? dt)
        {
            switch (ParentModel.Order.OrderBaseType)
            {
                case OrderBaseTypes.Current:
                    return new PaymentSchedule
                    {
                        EntryType = 0,
                        CustomerNo = ParentModel.Order.Sell_toCustomerNo,
                        DocumentNo = ParentModel.Order.No_,
                        Date = dt,
                        Amount = amount
                    };
                case OrderBaseTypes.Posted:
                    return new PostedPaymentSchedule
                    {
                        EntryType = 0,
                        CustomerNo = ParentModel.Order.Sell_toCustomerNo,
                        DocumentNo = ParentModel.Order.No_,
                        Date = dt,
                        Amount = amount
                    };
                default:
                    return new ReleasedPaymentSchedule
                    {
                        EntryType = 0,
                        CustomerNo = ParentModel.Order.Sell_toCustomerNo,
                        DocumentNo = ParentModel.Order.No_,
                        Date = dt,
                        Amount = amount
                    };
            }
        }

        public void DeleteCurrentRow()
        {
            if (SelectedSchedule != null)
            {
                if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                    (Schedules as List<PaymentSchedule>).Remove(SelectedSchedule as PaymentSchedule);
                else
                    (Schedules as List<ReleasedPaymentSchedule>).Remove(SelectedSchedule as ReleasedPaymentSchedule);
            }
        }

        public void DeleteRow(IPaymentSchedule sc)
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                (Schedules as List<PaymentSchedule>).Remove(sc as PaymentSchedule);
            else
                (Schedules as List<ReleasedPaymentSchedule>).Remove(sc as ReleasedPaymentSchedule);
        }

        public void UpdateModel()
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                ParentModel.PaymentSchedules = Schedules.Where(i => i.Amount.HasValue && i.Amount > 0).Cast<PaymentSchedule>().ToList();
            else
                ParentModel.PaymentSchedules = Schedules.Where(i => i.Amount.HasValue && i.Amount > 0).Cast<ReleasedPaymentSchedule>().ToList();
            ParentModel.RefreshSummary();
        }

        public decimal? MustPayeAmount
        {
            get
            {

                return ParentModel != null && ParentModel.Order != null && ParentModel.Order.AmountIncludingVat.HasValue
                    ? ParentModel.Order.AmountIncludingVat.Value - ParentModel.AmountSummaryPayment - AmountPayed
                    : 0;
            }
        }

        public Brush MustPayColor
        {
            get { return MustPayeAmount > 0 ? Brushes.DarkRed : Brushes.DarkGreen; }
        }

        public void RefreshSummary()
        {
            RaisePropertyChanged(() => MustPayeAmount);
            RaisePropertyChanged(() => MustPayColor);
        }
    }
}
