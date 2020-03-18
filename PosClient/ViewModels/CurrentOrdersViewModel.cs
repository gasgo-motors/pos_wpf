using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataLayer;
using DataLayer.Models;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class CurrentOrdersViewModel : PosViewModel
    {
        #region filter
        private string _no;
        public string No
        {
            get { return _no; }
            set
            {
                if (_no != value)
                {
                    _no = value;
                    RaisePropertyChanged(() => No);
                }
            }
        }

        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value;
                    RaisePropertyChanged(() => Code);
                }
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        private DateTime? _from;
        public DateTime? From
        {
            get { return _from; }
            set
            {
                if (_from != value)
                {
                    _from = value;
                    RaisePropertyChanged(() => From);
                }
            }
        }

        private DateTime? _to;
        public DateTime? To
        {
            get { return _to; }
            set
            {
                if (_to != value)
                {
                    _to = value;
                    RaisePropertyChanged(() => To);
                }
            }
        }
        #endregion


        public OrderBaseTypes OrderType { get; set; }


        public List<OrderShortEntry> OrdersList { get; set; }

        public decimal? Summary
        {
            get { return OrdersList == null ? null :  OrdersList.Sum(i => i.AmountIncludingVat); }
        } 


        public Visibility SendButtonVisibility
        {
            get { return OrderType == OrderBaseTypes.Current ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility SyncVisibility
        {
            get { return OrderType == OrderBaseTypes.Released ? Visibility.Visible : Visibility.Collapsed; }
        }

        private bool _checkAll;
        public bool CheckAll
        {
            get { return _checkAll; }
            set
            {
                if (value != _checkAll)
                {
                    _checkAll = value;

                    foreach (var i in OrdersList)
                    {
                        i.IsChecked = _checkAll;
                        //RaisePropertyChanged(() => i);
                    }
                    //var _orderList = OrdersList;
                    //OrdersList = null;
                    //OrdersList = _orderList;
                    //RaisePropertyChanged(() => OrdersList);

                }
            }
        }

        public void Refresh()
        {
            No = null;
            Code = null;
            Name = null;
            From = null;
            To = null;
            OrdersList =  DaoController.Current.GetOrdersList(OrderType);
            if (OrderType == OrderBaseTypes.Released)
                OrdersList = OrdersList.Where(i => i.DocumentType != 0).ToList();
            RaisePropertyChanged(() => OrdersList);
            RaisePropertyChanged(() => Summary);
            
            RaisePropertyChanged(() => SendButtonVisibility);
            RaisePropertyChanged(() => SyncVisibility);
        }

        public void Filter()
        {
            OrdersList = DaoController.Current.GetOrdersListByFilter(OrderType, _no, _code, _name, _from, _to);
            RaisePropertyChanged(() => OrdersList);
            RaisePropertyChanged(() => Summary);
        }

        public void ClearFilter()
        {
            Refresh();
        }


    }
}
