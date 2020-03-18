using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using BusinessLayer;
using DataLayer;
using DataLayer.Models;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class PaymentViewModel : PosViewModel
    {

        public OrderViewModel ParentModel { get; set; }
        public PaymentViewModel() { }

        public PaymentViewModel(OrderViewModel parentModel)
        {
            ParentModel = parentModel;
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                _geJournals = new List<GenJournalLine>();
            else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Released)
                _geJournals = new List<ReleasedGenJournalLine>();
            else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Posted)
                _geJournals = new List<PostedGenJournalLine>();
            foreach (var s in parentModel.GenJournalLines)
            {
                AddNewJournal(Clonejournal(s));
            }
        }

        public decimal ClientBalance
        {
            get
            {
                if (ParentModel.Order != null && ParentModel.Order.CurrentCustomer != null)
                {
                    return ParentModel.Order.CurrentCustomer.Balance.HasValue
                        ? Math.Round(ParentModel.Order.CurrentCustomer.Balance.Value, 2)
                        : 0;
                }
                return 0;
            }
        }

        public void Refresh()
        {
            RefreshSummary();
        }

        public decimal? AmountPayed
        {
            get
            {
                if (_geJournals == null) return 0;
                return _geJournals.Sum(i => i.Amount);
            }
        }

        public void UpdateAmountPayed()
        {
            RaisePropertyChanged(() => AmountPayed);
            RefreshSummary();
        }

        public void AddNewJournal(IGenJournalLine j)
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                (_geJournals as List<GenJournalLine>).Add(j as GenJournalLine);
            else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Released)
                (_geJournals as List<ReleasedGenJournalLine>).Add(j as ReleasedGenJournalLine);
            else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Posted)
                (_geJournals as List<PostedGenJournalLine>).Add(j as PostedGenJournalLine);
        }


        public IGenJournalLine Clonejournal(IGenJournalLine s)
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                return new GenJournalLine
                {
                    TemplateName = s.TemplateName,
                    BatchName = s.BatchName,
                    DocumentNo = s.DocumentNo,
                    DocumentType = s.DocumentType,
                    PostingDate = s.PostingDate,
                    AccountType = s.AccountType,
                    AccountNo_ = s.AccountNo_,
                    Bal_AccountType = s.Bal_AccountType,
                    Bal_AccountNo_ = s.Bal_AccountNo_,
                    Amount = s.Amount,
                    Salespers__Purch_Code = s.Salespers__Purch_Code,
                    ResponsibilityCenter = s.ResponsibilityCenter,
                    PaymentMethodCode = s.PaymentMethodCode
                };
            else if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Released)
                return new ReleasedGenJournalLine
                {
                    TemplateName = s.TemplateName,
                    BatchName = s.BatchName,
                    DocumentNo = s.DocumentNo,
                    PostingDate = s.PostingDate,
                    AccountType = s.AccountType,
                    AccountNo_ = s.AccountNo_,
                    Bal_AccountType = s.Bal_AccountType,
                    Bal_AccountNo_ = s.Bal_AccountNo_,
                    Amount = s.Amount,
                    Salespers__Purch_Code = s.Salespers__Purch_Code,
                    ResponsibilityCenter = s.ResponsibilityCenter,
                    PaymentMethodCode = s.PaymentMethodCode
                };
            else
                return new PostedGenJournalLine
                {
                    TemplateName = s.TemplateName,
                    BatchName = s.BatchName,
                    DocumentNo = s.DocumentNo,
                    PostingDate = s.PostingDate,
                    AccountType = s.AccountType,
                    AccountNo_ = s.AccountNo_,
                    Bal_AccountType = s.Bal_AccountType,
                    Bal_AccountNo_ = s.Bal_AccountNo_,
                    Amount = s.Amount,
                    Salespers__Purch_Code = s.Salespers__Purch_Code,
                    ResponsibilityCenter = s.ResponsibilityCenter,
                    PaymentMethodCode = s.PaymentMethodCode
                };
        }

        public IGenJournalLine GetNewRow()
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                return new GenJournalLine()
                {
                    TemplateName = App.Current.PosSetting.Settings_JnlTemplateName,
                    BatchName = App.Current.PosSetting.Settings_JnlBatchName,
                    DocumentNo = ParentModel.Order.No_,
                    DocumentType = ParentModel.Order.DocumentType,
                    PostingDate = DateTime.Now,
                    AccountType = (App.Current.User.UserType == PosUserTypes.Distributor || App.Current.User.UserType == PosUserTypes.PreSaler) ? 2 : 3,
                    AccountNo_ = (App.Current.User.UserType == PosUserTypes.Distributor || App.Current.User.UserType == PosUserTypes.PreSaler) ? App.Current.PosSetting.Settings_RespEmployee : null,
                    Bal_AccountType = 1,
                    Bal_AccountNo_ = ParentModel.Order.Sell_toCustomerNo,
                    Salespers__Purch_Code = App.Current.PosSetting.Settings_SalesPersonCode,
                    ResponsibilityCenter = App.Current.PosSetting.Settings_ResponsibilityCenter,
                    PaymentMethodCode = "CASH",
                    IsNew = true
                };
            else
                return new ReleasedGenJournalLine()
                {
                    TemplateName = App.Current.PosSetting.Settings_JnlTemplateName,
                    BatchName = App.Current.PosSetting.Settings_JnlBatchName,
                    DocumentNo = ParentModel.Order.No_,
                    PostingDate = DateTime.Now,
                    AccountType = (App.Current.User.UserType == PosUserTypes.Distributor || App.Current.User.UserType == PosUserTypes.PreSaler) ? 2 : 3,
                    AccountNo_ = (App.Current.User.UserType == PosUserTypes.Distributor || App.Current.User.UserType == PosUserTypes.PreSaler) ? App.Current.PosSetting.Settings_RespEmployee : null,
                    Bal_AccountType = 1,
                    Bal_AccountNo_ = ParentModel.Order.Sell_toCustomerNo,
                    Salespers__Purch_Code = App.Current.PosSetting.Settings_SalesPersonCode,
                    ResponsibilityCenter = App.Current.PosSetting.Settings_ResponsibilityCenter,
                    PaymentMethodCode = "CASH",
                    IsNew = true
                };
        }

        public void DeleteCurrentRow()
        {
            if (SelectedGeJournal != null)
            {
                if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                    (GeJournals as List<GenJournalLine>).Remove(SelectedGeJournal as GenJournalLine);
                else
                    (GeJournals as List<ReleasedGenJournalLine>).Remove(SelectedGeJournal as ReleasedGenJournalLine);
            }
        }

        public void DeleteRow(IGenJournalLine ln)
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                (GeJournals as List<GenJournalLine>).Remove(ln as GenJournalLine);
            else
                (GeJournals as List<ReleasedGenJournalLine>).Remove(ln as ReleasedGenJournalLine);
        }

        public void UpdateModel()
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                ParentModel.GenJournalLines = GeJournals.Where(i => i.Amount.HasValue && i.Amount > 0).Cast<GenJournalLine>().ToList();
            else
                ParentModel.GenJournalLines = GeJournals.Where(i => i.Amount.HasValue && i.Amount > 0).Cast<ReleasedGenJournalLine>().ToList();
            ParentModel.RefreshSummary();
        }


        private IEnumerable<IGenJournalLine> _geJournals;
        public IEnumerable<IGenJournalLine> GeJournals
        {
            get { return _geJournals; }
            set
            {
                if (value != _geJournals)
                {
                    _geJournals = value;
                    RaisePropertyChanged(() => GeJournals);
                }
            }
        }

        private IGenJournalLine _selectedGeJournal;
        public IGenJournalLine SelectedGeJournal
        {
            get { return _selectedGeJournal; }
            set
            {
                if (_selectedGeJournal != value)
                {
                    _selectedGeJournal = value;
                }
            }
        }


        public decimal? MustPayeAmount
        {
            get
            {

                return ParentModel != null && ParentModel.Order != null && ParentModel.Order.AmountIncludingVat.HasValue
                    ? ParentModel.Order.AmountIncludingVat.Value - ParentModel.AmountSummaryPaymentSchedule - AmountPayed
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
