using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class CurrentQuotesViewModel : PosViewModel
    {
        public void Refresh()
        {
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

        private List<ReleasedSalesLine> _pList;
        public List<ReleasedSalesLine> PList
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

        public Customer CurrentCustomer { get; set; }

        public CurrentQuotesViewModel()
        {

        }

        public CurrentQuotesViewModel(Customer customer)
        {
            CurrentCustomer = customer;
            CustomerName = customer.Name;
            PList = DaoController.Current.GetReleasedSalesLines(customer.No_);
        }
    }
}
