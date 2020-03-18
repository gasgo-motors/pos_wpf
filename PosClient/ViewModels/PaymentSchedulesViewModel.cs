using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using DataLayer.Models;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class PaymentSchedulesViewModel : PosViewModel
    {

        private List<PaymentSchedule> _pList;
        public List<PaymentSchedule> PList
        {
            get { return _pList; }
            set
            {
                if (value != _pList)
                {
                    _pList = value;
                    RaisePropertyChanged(() => PList);
                }
            }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                if (value != _customerName)
                {
                    _customerName = value;
                    RaisePropertyChanged(() => CustomerName);
                }
            }
        }

        public Customer CurrentCustomer { get; set; }

        public PaymentSchedulesViewModel()
        {
            
        }

        public PaymentSchedulesViewModel(Customer customer)
        {
            CurrentCustomer = customer;
            PList = DaoController.Current.GetPaymentSchedules(customer.No_);
            CustomerName = customer.Name;
        }

    }
}
