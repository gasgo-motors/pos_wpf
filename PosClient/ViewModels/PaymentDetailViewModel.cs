using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BusinessLayer;
using DataLayer;
using DataLayer.Models;
using PosClient.Helpers;
using PosClient.Views;

namespace PosClient.ViewModels
{
    public class PaymentDetailViewModel : PosViewModel
    {

        public Dictionary<int,string> AccountTypes { get; set; } 

        public decimal ClientBalance { get; set; }

        private string _paymentMethodCode;

        private IGenJournalLine _DbJournal;
        public string PaymentMethodCode
        {
            get { return _paymentMethodCode; }
            set
            {
                if (value != _paymentMethodCode)
                {
                    _paymentMethodCode = value;
                    Journal.PaymentMethodCode = _paymentMethodCode;
                    UpdatePaymentMethodCode();
                }
            }

        }

        private string _errorText;

        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                if (value != _errorText)
                {
                    _errorText = value;
                    RaisePropertyChanged(() => ErrorText);
                }
            }
        }


        private BankAccount _selectedAccount;
        public BankAccount SelectedAccount
        {
            get { return _selectedAccount; }
            set
            {
                if (value != _selectedAccount)
                {
                    _selectedAccount = value;
                    if (_selectedAccount == null)
                        Journal.AccountNo_ = null;
                    else
                        Journal.AccountNo_ = _selectedAccount.No_;
                    RaisePropertyChanged(() => SelectedAccount);
                }
            }
        }

        public bool PreSalerIsEnabled
        {
            get
            {
                return !(App.Current.User.UserType == PosUserTypes.PreSaler ||
                       App.Current.User.UserType == PosUserTypes.Distributor);
            }
        }

        public Visibility PresalerVisibility
        {
            get
            {
                return (App.Current.User.UserType == PosUserTypes.PreSaler ||
                        App.Current.User.UserType == PosUserTypes.Distributor)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public Visibility ShopVisibility
        {
            get
            {
                return (App.Current.User.UserType == PosUserTypes.PreSaler ||
                        App.Current.User.UserType == PosUserTypes.Distributor)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }


        private GenJournalLine _journal;
        public GenJournalLine Journal
        {
            get { return _journal; }
            set
            {
                if (value != _journal)
                {
                    _journal = value;
                    RaisePropertyChanged(() => Journal);
                }
            }
        }


        public Visibility RemoveVisibility
        {
            get { return _DbJournal != null &&  !_DbJournal.IsNew ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<BankAccount> Blist { get; set; }


        public PaymentDetailViewModel() { }


        public PaymentDetailViewModel(IGenJournalLine j, decimal? clientBalance)
        {
            _DbJournal = j;
            Journal = new GenJournalLine
            {
                AccountNo_ = j.AccountNo_,
                Amount = j.Amount,
                AccountType = j.AccountType,
                Bal_AccountType = j.Bal_AccountType,
                Bal_AccountNo_ = j.Bal_AccountNo_,
                PaymentMethodCode = j.PaymentMethodCode
            };
            _paymentMethodCode = j.PaymentMethodCode;
            if (Journal.PaymentMethodCode == "CASH")
            {
                Blist = DaoController.Current.GetBankAccounts(1, 0);
            }
            else
            {
                Blist = DaoController.Current.GetBankAccounts(0, 1);
            }
            _selectedAccount = Blist.FirstOrDefault(i => i.No_ == Journal.AccountNo_);
            if (Blist.Count == 1 && !( App.Current.User.UserType == PosUserTypes.Distributor || App.Current.User.UserType == PosUserTypes.PreSaler) )
            {
                _selectedAccount = Blist.FirstOrDefault();
                Journal.AccountNo_ = Blist.FirstOrDefault().No_;
            }

            AccountTypes = new Dictionary<int, string>()
            {
                {0, "G/L Account"},
                {1, "Customer"},
                {2, "Vendor"},
                {3, "Bank Account"}
            };
            ClientBalance = clientBalance.HasValue ? clientBalance.Value : 0;
            // RaisePropertyChanged(() => Journal);
        }

        public void UpdatePaymentMethodCode()
        {
            var blist = new List<BankAccount>();
            if (Journal.PaymentMethodCode == "CASH")
            {
                blist = DaoController.Current.GetBankAccounts(1, 0);
            }
            else
            {
                blist = DaoController.Current.GetBankAccounts(0, 1);
            }
            if (blist.Count == 1)
            {
                SelectedAccount = blist.First();
            }
            else
                SelectedAccount = null;
            Blist = blist;
            RaisePropertyChanged(() => Blist);
            
        }

        public string Save()
        {
            if (string.IsNullOrEmpty(Journal.AccountNo_))
            {
                ErrorText = "აირჩიეთ მიმღები!";
                return ErrorText;
            }
            if (!Journal.Amount.HasValue || Journal.Amount <= 0)
            {
                ErrorText = "შეიყვანეთ თანხა!";
                return ErrorText;
            }
            _DbJournal.AccountNo_ = Journal.AccountNo_;
            _DbJournal.Amount = Journal.Amount;
            _DbJournal.AccountType = Journal.AccountType;
            _DbJournal.Bal_AccountType = Journal.Bal_AccountType;
            _DbJournal.Bal_AccountNo_ = Journal.Bal_AccountNo_;
            _DbJournal.PaymentMethodCode = Journal.PaymentMethodCode;
            if (_DbJournal.IsGeneral == true)
            {
                DaoController.Current.CreateGeneralGurnalLine(_DbJournal as GenJournalLine);
            }
            else
            {
                if (_DbJournal.IsNew)
                {
                    Payment.Current.CurrentModel.AddNewJournal(_DbJournal);
                    _DbJournal.IsNew = false;
                }
            }
            return null;
        }

    }
}
