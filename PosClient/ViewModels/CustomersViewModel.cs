using DataLayer;
using LiveCharts;
using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using BusinessLayer;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace PosClient.ViewModels
{
    public class CustomersViewModel : PosViewModel
    {
        public List<Customer> PosCustomersBase { get; set; }
        public List<Customer> PosCustomers { get; set; }

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                if (value != _selectedCustomer)
                {
                    _selectedCustomer = value;
                    RaisePropertyChanged(() => SelectedCustomer);
                }
            }
        }

        public Visibility NonDistributorVisibility
        {
            get { return App.Current.User.UserType != PosUserTypes.Distributor ? Visibility.Visible : Visibility.Collapsed; }
        }

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                if (_filterString != value)
                {
                    _filterString = value;
                    RaisePropertyChanged(() => FilterString);
                    PosCustomers = FilterList();
                    RaisePropertyChanged(() => PosCustomers);
                }
            }
        }

        public string CurrentLocation
        {
            get
            {
                return App.Current.PosSetting.Settings_Location;
            }
            set
            {
                if (value != App.Current.PosSetting.Settings_Location)
                {
                    App.Current.PosSetting.Settings_Location = value;
                    SettingsManager.Current.UpdateSettings(App.Current.PosSetting);
                    RaisePropertyChanged(() => CurrentLocation);
                }
            }
        }

        public List<string> Locations { get; set; }

        public void InitLocations()
        {
            var list = App.Current.PosSetting.Settings_LocationAll.Split(',').ToList();
            list.Add(App.Current.PosSetting.Settings_Location);
            Locations = list.Distinct().ToList();
            RaisePropertyChanged(() => Locations);
        }


        private string _filterStringSN;
        public string FilterStringSN
        {
            get { return _filterStringSN; }
            set
            {
                if (_filterStringSN != value)
                {
                    _filterStringSN = value;
                    RaisePropertyChanged(() => FilterStringSN);
                    PosCustomers = FilterList();
                    RaisePropertyChanged(() => PosCustomers);
                }
            }
        }

        private CustomerButtonTypes _customerButtonType;
        public CustomerButtonTypes CustomerButtonType
        {
            get { return _customerButtonType; }
            set
            {
                if (value != _customerButtonType)
                {
                    _customerButtonType = value;
                    RaisePropertyChanged(() => IsBtnCurrentListEnabled);
                    RaisePropertyChanged(() => IsBtnAllListEnabled);
                    Refresh();
                }
            }
        }

        public bool IsBtnCurrentListEnabled
        {
            get { return _customerButtonType == CustomerButtonTypes.All; }
        }

        public bool IsBtnAllListEnabled
        {
            get { return _customerButtonType == CustomerButtonTypes.Today; }
        }


        public void Refresh()
        {
            _filterString = "";
            _filterStringSN = "";
            RaisePropertyChanged(() => FilterString);
            RaisePropertyChanged(() => FilterStringSN);
            RaisePropertyChanged(() => NonDistributorVisibility);
            PosCustomersBase =
                DaoController.Current.GetCustomers()
                    .Where(
                        i =>
                            string.IsNullOrEmpty(_filterString) ||
                            (!string.IsNullOrEmpty(i.Name) && i.Name.ToLower().Contains(_filterString.ToLower()))).ToList();
            if (CustomerButtonType == CustomerButtonTypes.Today)
            {
                var c = ((int)DateTime.Today.DayOfWeek).ToString();
                PosCustomersBase = PosCustomersBase.Where(i => i.VisitWeekDays != null && i.VisitWeekDays.Contains(c)).ToList();
            }
            PosCustomers = FilterList();
            RaisePropertyChanged(() => PosCustomers);

            // InitCharts();
            InitChartsTable();
            InitLocations();
        }

        public void InitCharts()
        {
            var list = DaoController.Current.GetRegionBudgetActual();
            SeriesCollection = new SeriesCollection();
            foreach (var r in list)
            {
                if (r.BudgetAmount == null) r.BudgetAmount = 0;
                if (r.ActualAmount == null) r.ActualAmount = 0;
                double val = r.BudgetAmount == 0 ? 0 : Math.Round(((double)(r.ActualAmount.Value) / (double)r.BudgetAmount.Value), 3, MidpointRounding.AwayFromZero);

                SeriesCollection.Add(new PieSeries
                {
                    Title = r.RegionName,
                    Values = new ChartValues<ObservableValue> { new ObservableValue(val) },
                    DataLabels = true
                });
            }
            RaisePropertyChanged(() => SeriesCollection);


            var budget = list.Sum(i => i.BudgetAmount);
            var actual = list.Sum(i => i.ActualAmount);
            if (budget == null) budget = 0;
            if (actual == null) actual = 0;
            double val1 = budget == 0 ? 0 : Math.Round(((double)(actual.Value) / (double)budget.Value), 3, MidpointRounding.AwayFromZero);
            double val2 = 1 - val1;
            SeriesCollection1 = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "",
                    Values = new ChartValues<ObservableValue> {new ObservableValue(val1)},
                    DataLabels = false,
                    Fill = Brushes.LightSkyBlue
                },
                new PieSeries
                {
                    Title = "",
                    Values = new ChartValues<ObservableValue> {new ObservableValue(val2)},
                    DataLabels = false,
                    Fill = Brushes.Black
                }
            };
            RaisePropertyChanged(() => SeriesCollection1);
        }

        public List<RegionBudgetActual> RegionsList { get; set; }

        public void InitChartsTable()
        {
            RegionsList = DaoController.Current.GetRegionBudgetActual();
            RaisePropertyChanged(() => RegionsList);
        }

        public List<Customer> FilterList()
        {
            return PosCustomersBase.Where(
                    i =>
                        (string.IsNullOrEmpty(_filterString) ||
                        (!string.IsNullOrEmpty(i.Name) && i.Name.ToLower().Contains(_filterString.ToLower()))) &&
                        (string.IsNullOrEmpty(_filterStringSN) ||
                        (!string.IsNullOrEmpty(i.VATRegistrationNo_) && i.VATRegistrationNo_.ToLower().Contains(_filterStringSN.ToLower())))).ToList();
        }


        public SalesHeader CreateNewSalesHeader()
        {
            if (SelectedCustomer == null) return null;
            var header = new SalesHeader
            {
                No_ = DaoController.Current.GenerateNewKey(App.Current.PosSetting.Settings_SalesHeaderNumberCount),
                DocumentType = 1,
                PostingDate = DateTime.Now,
                Sell_toCustomerNo = SelectedCustomer.No_,
                Sell_toCustomerName = SelectedCustomer.Name,
                CustomerPriceGroup = SelectedCustomer.CustomerPriceGroup,
                SalespersonCode = App.Current.PosSetting.Settings_SalesPersonCode,
                SalesLines = new List<SalesLine>(),
                JournalLines = new List<GenJournalLine>(),
                PaymentSchedules = new List<PaymentSchedule>(),
                CurrentCustomer = SelectedCustomer
            };
            return header;
        }


        public SeriesCollection SeriesCollection { get; set; }

        public SeriesCollection SeriesCollection1 { get; set; }
    }

    public enum CustomerButtonTypes
    {
        Today,
        All
    }
}
