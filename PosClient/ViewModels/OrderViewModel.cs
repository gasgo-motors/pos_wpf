using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using DataLayer;
using DataLayer.Models;
using PosClient.Helpers;
using BusinessLayer;
using WpfControls.Editors;

namespace PosClient.ViewModels
{
    public class OrderViewModel : PosViewModel
    {

        public static string Sell_toCustomerNo;

        public Customer CurrentCustomer
        {
            get
            {
                return Order?.CurrentCustomer;
            }
            set
            {
                if(value != null &&  value.No_ != Order.CurrentCustomer.No_)
                {
                    Order.CurrentCustomer = value;
                    Order.Sell_toCustomerNo = value.No_;
                    Order.Sell_toCustomerName = value.Name;
                    Order.CustomerPriceGroup = value.CustomerPriceGroup;
                    RaisePropertyChanged(() => CurrentCustomer);
                    OrderViewModel.Sell_toCustomerNo = Order.Sell_toCustomerNo;
                    ShipToAddresses = DaoController.Current.GetShipToAddresses(Order.Sell_toCustomerNo);
                    if (ShipToAddresses.Count == 1)
                        SelectedAddress = ShipToAddresses[0];
                }
            }
        }


        private bool _isNew;
        public bool IsNew
        {
            get { return _isNew; }
            set
            {
                if (value != _isNew)
                {
                    _isNew = value;
                    RaisePropertyChanged(() => BogBackButtonVisibility);
                }
            }
        }

        private bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                if (value != _isEditable)
                {
                    // hghf
                    _isEditable = value;
                    RaisePropertyChanged(() => SaveAndCancelButtonsVisibility);
                    RaisePropertyChanged(() => SaveAndSendButtonsVisibility);
                    RaisePropertyChanged(() => CancelButtonsVisibility);
                    RaisePropertyChanged(() => QueteButtonsVisibility);
                    RaisePropertyChanged(() => EditButttonVisibility);
                    RaisePropertyChanged(() => IsEditable);
                    RaisePropertyChanged(() => IsReadOnly);
                    RaisePropertyChanged(() => IsReadOnlyRow);
                }
            }
        }

        public bool IsReadOnly
        {
            get { return !IsEditable; }
        }

        public bool IsReadOnlyRow
        {
            get
            {
                return !IsEditable || Order.OrderBaseType != OrderBaseTypes.Current;
            }
        }

        public void LoadOrderFromNav()
        {
            if (Order.OrderBaseType == OrderBaseTypes.Released)
            {
                var navOrder = OrdersManager.Current.GetNavOrder("Order", Order.No_);
            }
        }

        public Visibility BogBackButtonVisibility
        {
            get { return !_isNew ? Visibility.Visible : Visibility.Collapsed; }
        }


        public Visibility BarCodeVisibility
        {
            get { return Order == null || ((App.Current.User.UserType == PosUserTypes.Shop) && (Order.OrderBaseType == OrderBaseTypes.Current)) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility EditButttonVisibility
        {
            get { return Order != null && Order.OrderBaseType != OrderBaseTypes.Posted && !_isEditable ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility SaveAndCancelButtonsVisibility
        {
            get { return IsEditable ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility SaveAndSendButtonsVisibility
        {
            get { return IsEditable && (Order.OrderBaseType == OrderBaseTypes.Current) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility DeleteButtonVisibility
        {
            get { return IsEditable ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility CancelButtonsVisibility
        {
            get { return IsEditable && (App.Current.User.UserType == PosUserTypes.Manager) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility QueteButtonsVisibility
        {
            get { return IsEditable && !IsNew ? Visibility.Visible : Visibility.Collapsed; }
        }


        private ISalesHeader _order;
        public ISalesHeader Order
        {
            get { return _order; }
            set
            {
                if (value != _order)
                {
                    _order = value;
                    RaisePropertyChanged(() => Order);
                    RaisePropertyChanged(() => EditButttonVisibility);
                }
            }
        }

        public decimal? AmountSummaryPayment
        {
            get
            {
                if (GenJournalLines == null) return 0;
                return GenJournalLines.Sum(i => i.Amount);
            }
        }

        public decimal? AmountSummaryPaymentSchedule
        {
            get
            {
                if (PaymentSchedules == null) return 0;
                return PaymentSchedules.Sum(i => i.Amount);
            }
        }

        public decimal? MustPayeAmount
        {
            get
            {
                return Order != null && Order.AmountIncludingVat.HasValue
                    ? Order.AmountIncludingVat.Value - AmountSummaryPaymentSchedule - AmountSummaryPayment
                    : 0;
            }
        }



        private string _recievedAmount;
        public string RecievedAmount
        {
            get { return _recievedAmount; }
            set
            {
                if (value != _recievedAmount)
                {
                    _recievedAmount = value;
                    RaisePropertyChanged(() => RecievedAmount);
                    RaisePropertyChanged(() => RemainingAmount);

                }
            }
        }

        public decimal? RemainingAmount
        {
            get
            {
                decimal d = 0;
                if (decimal.TryParse(RecievedAmount, out d) && Order != null)
                    return Order.AmountIncludingVat - d;
                return null;
            }
        }

        public Brush MustPayColor
        {
            get { return MustPayeAmount > 0 ? Brushes.DarkRed : Brushes.DarkGreen; }
        }

        public Visibility MustPayVisibility
        {
            get { return Order != null && Order.OrderBaseType != OrderBaseTypes.Posted ? Visibility.Visible : Visibility.Collapsed; }
        }


        private string _selectedOeNumber;
        public string SelectedOeNumber
        {
            get { return _selectedOeNumber; }
            set
            {
                if (value != _selectedOeNumber)
                {
                    _selectedOeNumber = value;
                    RaisePropertyChanged(() => SelectedOeNumber);
                }
            }
        }

        public Visibility PaymentButtonVisibility
        {
            get
            {
                return Order == null || Order.Sell_toCustomerNo == App.Current.PosSetting.Settings_GenCustomerCode
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }


        private List<Ship_to_Address> _shipToAddresses;
        public List<Ship_to_Address> ShipToAddresses
        {
            get { return _shipToAddresses; }
            set
            {
                if (value != _shipToAddresses)
                {
                    _shipToAddresses = value;
                    RaisePropertyChanged(() => ShipToAddresses);
                }
            }
        }

        private Ship_to_Address _selectedAddress;
        public Ship_to_Address SelectedAddress
        {
            get { return _selectedAddress; }
            set
            {
                if (value != _selectedAddress)
                {
                    _selectedAddress = value;
                    if (_selectedAddress != null)
                    {
                        Order.Ship_toAddressCode = _selectedAddress.Code;
                        Order.Ship_toAddress = _selectedAddress.Name;
                    }
                    else
                    {
                        Order.Ship_toAddressCode = null;
                        Order.Ship_toAddress = null;
                    }
                    RaisePropertyChanged(() => SelectedAddress);
                }
            }
        }


        private List<Vendor> _vendors;
        public List<Vendor> Vendors
        {
            get { return _vendors; }
            set
            {
                if (value != _vendors)
                {
                    _vendors = value;
                    RaisePropertyChanged(() => Vendors);
                }
            }
        }


        private List<Vehicle> _vehicles;
        public List<Vehicle> Vehicles
        {
            get { return _vehicles; }
            set
            {
                if (value != _vehicles)
                {
                    _vehicles = value;
                    RaisePropertyChanged(() => Vehicles);
                }
            }
        }


        public void RefreshSummary()
        {
            RaisePropertyChanged(() => Order);
            RaisePropertyChanged(() => AmountSummaryPayment);
            RaisePropertyChanged(() => AmountSummaryPaymentSchedule);
            RaisePropertyChanged(() => MustPayeAmount);
            RaisePropertyChanged(() => MustPayColor);
            RaisePropertyChanged(() => MustPayVisibility);
        }

        public void AddItemByBarCode(string code)
        {
            string unitofmeasure;
            Item item;
            DaoController.Current.GetItemByBarCode(code, out item, out unitofmeasure);
            if (item != null)
            {

                var sl = SalesLines.FirstOrDefault(i => i.No_ == item.No_ && i.OrderType == 0 && i.UnitOfMeasureCode == unitofmeasure);
                if (sl == null)
                {
                    decimal? price = DaoController.Current.GetsalesPrice(item.No_, unitofmeasure, Order.Sell_toCustomerNo,
                        Order.CurrentCustomer.CustomerPriceGroup);
                    if (price == null)
                        price = item.UnitPrice;
                    var ind = SalesLines != null && SalesLines.Count() > 0 ? SalesLines.Max(i => i.LineNo_) + 1 : 1;
                    var nl = new SalesLine
                    {
                        DocumentNo_ = Order.No_,
                        DocumentType = 1,
                        LineNo_ = ind,
                        Type = 2,
                        No_ = item.No_,
                        Description = item.Description,
                        Quantity = 1,
                        UnitPrice = price,
                        Sell_toCustomerNo = Order.Sell_toCustomerNo,
                        LocationCode = App.Current.PosSetting.Settings_Location,
                        AmountIncludingVAT = Math.Round(price.Value, 2, MidpointRounding.AwayFromZero),
                        UnitOfMeasureCode = unitofmeasure,
                        OrderType = (int)OrderTypes.Ok,
                        LineDiscountAmount = 0,
                        LineDiscountPercent = 0
                    };
                    (SalesLines as List<SalesLine>).Add(nl);
                }
                else
                {
                    sl.Quantity = sl.Quantity + 1;
                    sl.AmountIncludingVAT = Math.Round(sl.Quantity.Value * sl.UnitPrice.Value, 2, MidpointRounding.AwayFromZero);
                }
                Order.AmountIncludingVat = SalesLines.Sum(i => i.AmountIncludingVAT);
                RefreshGrid();
                RefreshSummary();
            }
        }


        private IEnumerable<ISalesLine> _salesLines;
        public IEnumerable<ISalesLine> SalesLines
        {
            get { return _salesLines; }
            set
            {
                if (value != _salesLines)
                {
                    _salesLines = value;
                    RaisePropertyChanged(() => SalesLinesEntries);
                }
            }
        }

        public List<SalesLineShortEntry> SalesLinesEntries
        {
            get
            {
                if (_salesLines == null) return null;
                return _salesLines.Select(i => new SalesLineShortEntry(i, this, Vendors)).ToList();
            }
        }

        public void DeleteSalesLine(int line_no)
        {
            if (Order.OrderBaseType == OrderBaseTypes.Released)
            {
                var sl = ((List<ReleasedSalesLine>)_salesLines).FirstOrDefault(i => i.LineNo_ == line_no);
                if (sl != null)
                    ((List<ReleasedSalesLine>)_salesLines).Remove(sl);
            }
            else
            {
                var sl = ((List<SalesLine>)_salesLines).FirstOrDefault(i => i.LineNo_ == line_no);
                if (sl != null)
                    ((List<SalesLine>)_salesLines).Remove(sl);
            }
            Order.AmountIncludingVat = SalesLines.Sum(i => i.AmountIncludingVAT);
            RefreshSummary();
            // RaisePropertyChanged(() => SalesLines);
        }

        public IEnumerable<IPaymentSchedule> PaymentSchedules { get; set; }
        public IEnumerable<IGenJournalLine> GenJournalLines { get; set; }

        public OrderViewModel() { }

        public OrderViewModel(ISalesHeader order, bool isNew = false)
        {
            _order = order;
            IsNew = isNew;
            if (!isNew)
            {
                _salesLines = order.SalesLines;
                PaymentSchedules = order.PaymentSchedules;
                GenJournalLines = order.JournalLines;
            }
            else
            {
                _salesLines = new List<SalesLine>();
                PaymentSchedules = new List<PaymentSchedule>();
                GenJournalLines = new List<GenJournalLine>();
                IsEditable = true;
            }
        }

        public void Refresh()
        {
            OrderViewModel.Sell_toCustomerNo = Order.Sell_toCustomerNo;
            Vendors = DaoController.Current.GetVendors();
            Vehicles = DaoController.Current.GetCustomerVehicles(Order.Sell_toCustomerNo);
            ShipToAddresses = DaoController.Current.GetShipToAddresses(Order.Sell_toCustomerNo);
            if (ShipToAddresses.Count == 1)
                SelectedAddress = ShipToAddresses[0];
            RaisePropertyChanged(() => BarCodeVisibility);
            RaisePropertyChanged(() => PaymentButtonVisibility);
            RefreshGrid();
        }

        public void CreateOrder()
        {
            if (Order.OrderBaseType == OrderBaseTypes.Current)
            {
                if (App.Current.User.UserType == PosUserTypes.Shop &&
                    AmountSummaryPayment + AmountSummaryPaymentSchedule != Order.AmountIncludingVat &&
                    App.Current.PosSetting.Settings_GenCustomerCode == Order.Sell_toCustomerNo
                    )
                {
                    var gn = (GenJournalLines as List<GenJournalLine>).FirstOrDefault(i => i.PaymentMethodCode == "CASH");
                    if (gn != null)
                        gn.Amount += Order.AmountIncludingVat - AmountSummaryPayment - AmountSummaryPaymentSchedule;
                    else
                    {
                        gn = new GenJournalLine()
                        {
                            TemplateName = App.Current.PosSetting.Settings_JnlTemplateName,
                            BatchName = App.Current.PosSetting.Settings_JnlBatchName,
                            DocumentNo = Order.No_,
                            DocumentType = Order.DocumentType,
                            PostingDate = DateTime.Now,
                            AccountType =
                                (App.Current.User.UserType == PosUserTypes.Distributor ||
                                 App.Current.User.UserType == PosUserTypes.PreSaler)
                                    ? 2
                                    : 3,
                            AccountNo_ = DaoController.Current.GetBankAccounts(1, 0).First().No_,
                            Bal_AccountType = 1,
                            Bal_AccountNo_ = Order.Sell_toCustomerNo,
                            Salespers__Purch_Code = App.Current.PosSetting.Settings_SalesPersonCode,
                            ResponsibilityCenter = App.Current.PosSetting.Settings_ResponsibilityCenter,
                            PaymentMethodCode = "CASH",
                            Amount = Order.AmountIncludingVat - AmountSummaryPayment - AmountSummaryPaymentSchedule
                        };
                        (GenJournalLines as List<GenJournalLine>).Add(gn);
                    }
                }
                var sLines = (List<SalesLine>)SalesLines;
                if( sLines.Any(s => s.PostingDate.HasValue && s.PostingDate.Value.Date > Order.PostingDate.Value.Date ))
                {
                    var dates = sLines.Where(s => s.PostingDate.HasValue && s.PostingDate.Value.Date > Order.PostingDate.Value.Date).Select(s => s.PostingDate.Value.Date).Distinct();
                    foreach(var dt in dates)
                    {
                            var newHeader = new SalesHeader
                            {
                                No_ = DaoController.Current.GenerateNewKey(App.Current.PosSetting.Settings_SalesHeaderNumberCount),
                                DocumentType = 1,
                                PostingDate = dt,
                                Sell_toCustomerNo = Order.Sell_toCustomerNo,
                                Sell_toCustomerName = Order.Sell_toCustomerName,
                                CustomerPriceGroup = Order.CustomerPriceGroup,
                                SalespersonCode = App.Current.PosSetting.Settings_SalesPersonCode,
                                SalesLines = new List<SalesLine>(),
                                JournalLines = new List<GenJournalLine>(),
                                PaymentSchedules = new List<PaymentSchedule>(),
                                CurrentCustomer = Order.CurrentCustomer
                            };
                        var newSalesLines = sLines.Where(s => s.PostingDate.HasValue && s.PostingDate.Value.Date == dt).ToList();
                        newSalesLines.ForEach(n => { n.DocumentNo_ = newHeader.No_; });
                        newHeader.AmountIncludingVat = newSalesLines.Sum(i => i.AmountIncludingVAT);
                        DaoController.Current.CreateOrder(newHeader, newSalesLines,
                            new List<PaymentSchedule>(), new List<GenJournalLine>(), DateTime.Now );
                    }
                }
                sLines = sLines.Where(s => !s.PostingDate.HasValue || s.PostingDate.Value.Date == Order.PostingDate.Value.Date).ToList();
                Order.AmountIncludingVat = sLines.Sum(i => i.AmountIncludingVAT);
                DaoController.Current.CreateOrder((SalesHeader)Order, sLines,
                    (List<PaymentSchedule>)PaymentSchedules, (List<GenJournalLine>)GenJournalLines, DateTime.Now);
            }
            else
            {
                var rOrder = (ReleasedSalesHeader)Order;
                if (OrdersManager.Current.IsOrderWaybillUploaded(rOrder.DocumentType, rOrder.No_))
                {

                    rOrder.SalesLines = ((List<ReleasedSalesLine>)SalesLines).Where(i => !i.IsNew).ToList();
                    rOrder.PaymentSchedules = (List<ReleasedPaymentSchedule>)PaymentSchedules;
                    Order.JournalLines = (List<ReleasedGenJournalLine>)GenJournalLines;
                    OrdersManager.Current.SaveNavOrder(rOrder);
                    DaoController.Current.CreateReleaseOrder((ReleasedSalesHeader)Order,
                        rOrder.SalesLines, (List<ReleasedPaymentSchedule>)PaymentSchedules,
                        (List<ReleasedGenJournalLine>)GenJournalLines);

                    var oNo =
                        DaoController.Current.GenerateNewKey(App.Current.PosSetting.Settings_SalesHeaderNumberCount);
                    int mt = 0;
                    DaoController.Current.CreateOrder(
                        new SalesHeader
                        {
                            No_ = oNo,
                            DocumentType = 1,
                            PostingDate = DateTime.Now,
                            Sell_toCustomerNo = rOrder.Sell_toCustomerNo,
                            Sell_toCustomerName = rOrder.Sell_toCustomerName,
                            CustomerPriceGroup = rOrder.CustomerPriceGroup,
                            SalespersonCode = App.Current.PosSetting.Settings_SalesPersonCode,
                            SalesLines = new List<SalesLine>(),
                            JournalLines = new List<GenJournalLine>(),
                            PaymentSchedules = new List<PaymentSchedule>(),
                            CurrentCustomer = Order.CurrentCustomer
                        }, ((List<ReleasedSalesLine>)SalesLines).Where(i => i.IsNew).Select(i => new SalesLine
                        {
                            DocumentNo_ = oNo,
                            DocumentType = 1,
                            LineNo_ = ++mt,
                            Type = 2,
                            No_ = i.No_,
                            Description = i.Description,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            Sell_toCustomerNo = i.Sell_toCustomerNo,
                            LocationCode = App.Current.PosSetting.Settings_Location,
                            AmountIncludingVAT = i.AmountIncludingVAT,
                            UnitOfMeasureCode = i.UnitOfMeasureCode,
                            OrderType = i.OrderType,
                            LineDiscountAmount = 0,
                            LineDiscountPercent = 0,
                            LargeDescription = i.LargeDescription,
                            Service_Provider = i.Service_Provider,
                            Customer_Vehicle = i.Customer_Vehicle
                        }).ToList(),
                        new List<PaymentSchedule>(), new List<GenJournalLine>(), DateTime.Now);

                }
                else
                {
                    rOrder.SalesLines = (List<ReleasedSalesLine>)SalesLines;
                    rOrder.PaymentSchedules = (List<ReleasedPaymentSchedule>)PaymentSchedules;
                    Order.JournalLines = (List<ReleasedGenJournalLine>)GenJournalLines;
                    OrdersManager.Current.SaveNavOrder(rOrder);
                    DaoController.Current.CreateReleaseOrder((ReleasedSalesHeader)Order,
                        (List<ReleasedSalesLine>)SalesLines, (List<ReleasedPaymentSchedule>)PaymentSchedules,
                        (List<ReleasedGenJournalLine>)GenJournalLines);
                }

            }
        }

        public decimal ClientBalance
        {
            get
            {
                if (Order != null && Order.CurrentCustomer != null)
                {
                    return Order.CurrentCustomer.Balance.HasValue
                        ? Math.Round(Order.CurrentCustomer.Balance.Value, 2)
                        : 0;
                }
                return 0;
            }
        }

        public Visibility ClientBalancVisibility
        {
            get { return Order != null && Order.CurrentCustomer != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void RefreshGrid()
        {
            RaisePropertyChanged(() => SalesLinesEntries);
            PosClient.Views.Order.Current.gridsalesLInes.Items.Refresh();
        }




        public void QueteOrders()
        {
            foreach (var i in _salesLines)
            {
                i.OrderType = (int)OrderTypes.Q;
                i.AmountIncludingVAT = 0;
            }
            Order.AmountIncludingVat = 0;
            CreateOrder();
        }

    }

    public class SalesLineShortEntry : INotifyPropertyChanged
    {
        private ISalesLine _salesLine;
        private OrderViewModel _parentModel;

        public List<Vendor> Vendors { get; set; }

        public SalesLineShortEntry(ISalesLine salesLine, OrderViewModel parentModel, List<Vendor> vendors)
        {
            _salesLine = salesLine;
            _parentModel = parentModel;
            Vendors = vendors;
        }

        public string Description
        {
            get { return string.IsNullOrEmpty(_salesLine.LargeDescription) ? _salesLine.Description : _salesLine.LargeDescription; }
        }

        public decimal? Quantity
        {
            get { return _salesLine.Quantity; }
            set
            {
                if (_salesLine.Quantity != value)
                {

                    _salesLine.Quantity = value;
                    if (_salesLine.Quantity == null) _salesLine.Quantity = 0;
                    _salesLine.AmountIncludingVAT = (_salesLine.OrderType != 0 && _salesLine.OrderType != 3)
                        ? 0
                        : Math.Round(_salesLine.Quantity.Value * _salesLine.UnitPrice.Value, 2, MidpointRounding.AwayFromZero);

                    PropertyChanged(this, new PropertyChangedEventArgs("Quantity"));
                    PropertyChanged(this, new PropertyChangedEventArgs("AmountIncludingVAT"));
                    _parentModel.Order.AmountIncludingVat = _parentModel.SalesLines.Sum(i => i.AmountIncludingVAT);
                    _parentModel.RefreshSummary();
                }
            }
        }

        public decimal? UnitPrice
        {
            get { return _salesLine.UnitPrice; }
        }

        public decimal? AmountIncludingVAT
        {
            get { return _salesLine.AmountIncludingVAT; }
        }

        public string OrderTypeString
        {
            get { return _salesLine.OrderTypeString; }
        }


        public DateTime? PostingDate
        {
            get { return _salesLine.PostingDate.HasValue ? _salesLine.PostingDate : _parentModel.Order.PostingDate;  }
            set
            {
                if (_salesLine.PostingDate != value)
                {
                    _salesLine.PostingDate = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("PostingDate"));
                }
            }
        }

        public int LineNo
        {
            get { return _salesLine.LineNo_; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Visibility DeleteButtonVisibility
        {
            get
            {
                return _parentModel.IsEditable && ((_salesLine is SalesLine) ||
                       ((_salesLine is ReleasedSalesLine) && (_salesLine as ReleasedSalesLine).IsNew)) ? Visibility.Visible :
                Visibility.Collapsed;
            }
        }

        public Visibility ReadOnlyVisibility
        {
            get
            {
                return DeleteButtonVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        public string Service_Provider
        {
            get { return _salesLine.Service_Provider; }
            set
            {
                if (_salesLine.Service_Provider != value)
                {
                    _salesLine.Service_Provider = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Service_Provider"));
                }
            }
        }

        public string Service_Provider_No
        {
            get
            {
                var vendor = Vendors.FirstOrDefault(i => i.No_ == _salesLine.Service_Provider);
                return vendor != null ? vendor.Name : Service_Provider;
            }
        }

        public Vendor Service_ProviderObject
        {
            get
            {
                var vendor = Vendors.FirstOrDefault(i => i.No_ == _salesLine.Service_Provider);
                return vendor;
            }
            set
            {
                if (value != null && _salesLine.Service_Provider != value.No_)
                {
                    _salesLine.Service_Provider = value.No_;
                    PropertyChanged(this, new PropertyChangedEventArgs("Service_Provider"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Service_ProviderObject"));
                }
            }
        }

        public string Customer_Vehicle
        {
            get { return _salesLine.Customer_Vehicle; }
            set
            {
                if (_salesLine.Customer_Vehicle != value)
                {
                    _salesLine.Customer_Vehicle = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("Customer_Vehicle"));
                }
            }
        }

        private string _customer_Vehicle_Text;
        public string Customer_Vehicle_Text
        {
            get { return _customer_Vehicle_Text; }
            set
            {
                if (_customer_Vehicle_Text != value)
                {
                    _customer_Vehicle_Text = value;
                    if (_salesLine.Customer_Vehicle == null) _salesLine.Customer_Vehicle = _customer_Vehicle_Text;
                    // PropertyChanged(this, new PropertyChangedEventArgs("Customer_Vehicle"));
                }
            }
        }


    }

    public class OeNumbersSuggestionProvider : ISuggestionProvider
    {

        public System.Collections.IEnumerable GetSuggestions(string filter)
        {
            if (string.IsNullOrEmpty(filter) || filter.Length < 3)
            {
                return null;
            }
            return DaoController.Current.GetOeNumbers(filter);
        }

    }

    public class VendorsSuggestionProvider : ISuggestionProvider
    {
        public List<Vendor> Vendors { get; set; }


        public System.Collections.IEnumerable GetSuggestions(string filter)
        {
            if (Vendors == null)
                Vendors = DaoController.Current.GetVendors();
            if (filter == null) filter = string.Empty;
            return Vendors.Where(i => i.Name.Contains(filter)).ToList();
        }

    }

    public class VehiclesSuggestionProvider : ISuggestionProvider
    {
        public List<Vehicle> Vehicles { get; set; }


        public System.Collections.IEnumerable GetSuggestions(string filter)
        {
            Vehicles = DaoController.Current.GetCustomerVehicles(OrderViewModel.Sell_toCustomerNo);
            if (filter == null) filter = string.Empty;
            return Vehicles.Where(i => i.Vehicle_No_.Contains(filter)).Select(i => i.Vehicle_No_).ToList();
        }

    }


    public class CustomersProvider : ISuggestionProvider
    {
        public List<Customer> Customers { get; set; }


        public System.Collections.IEnumerable GetSuggestions(string filter)
        {
            if (Customers == null)
                Customers = DaoController.Current.GetCustomers();
            if (filter == null) filter = string.Empty;

            return Customers.Where(
                    i => i.Name.ToLower().Contains(filter.ToLower()) || i.VATRegistrationNo_.Contains(filter)).ToList();
        }

    }






}
