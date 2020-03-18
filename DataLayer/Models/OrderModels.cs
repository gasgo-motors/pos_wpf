using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Annotations;

namespace DataLayer.Models
{
    public class OrderShortEntry : INotifyPropertyChanged
    {
        public string No_ { get; set; }
        public int DocumentType { get; set; }
        public DateTime? PostingDate { get; set; }
        public string Sell_toCustomerNo { get; set; }
        public string Sell_toCustomerName { get; set; }
        public decimal? AmountIncludingVat { get; set; }
        public OrderBaseTypes OrderType { get; set; }




        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

    }

    public enum OrderBaseTypes
    {
        Current,
        Released,
        Posted
    }
}
