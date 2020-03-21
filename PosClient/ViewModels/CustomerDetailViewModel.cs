﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using BusinessLayer;
using DataLayer;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class CustomerDetailViewModel : PosViewModel
    {

        private List<Country> _countries;
        public List<Country> Countries
        {
            get { return _countries; }
            set
            {
                if (_countries != value)
                {
                    _countries = value;
                    RaisePropertyChanged(() => Countries);
                }
            }
        }

        private List<PostCodeCity> _cities;
        public List<PostCodeCity> Cities
        {
            get { return _cities; }
            set
            {
                if (_cities != value)
                {
                    _cities = value;
                    RaisePropertyChanged(() => Cities);
                }
            }
        }

        private bool _isNew;

        private Customer _currentCustomer;
        public Customer CurrentCustomer
        {
            get { return _currentCustomer; }
            set
            {
                if (_currentCustomer != value)
                {
                    _currentCustomer = value;
                    RaisePropertyChanged(() => CurrentCustomer);
                }
            }
        }

        public void SetName(string name)
        {
            _currentCustomer.Name = name;
        }

        private string _error;
        public string Error
        {
            get { return _error; }
            set
            {
                if (_error != value)
                {
                    _error = value;
                    RaisePropertyChanged(() => Error);
                }
            }
        }

        public bool IsNew
        {
            get { return _isNew; }
        }

        public Visibility BtnCloseVisibility
        {
            get { return _isNew ? Visibility.Collapsed : Visibility.Visible; }
        }


        public bool HasOrders
        {
            get { return DaoController.Current.HasClientOrders(_currentCustomer.No_); }
        }

        public void RemoveCusetomer()
        {
            DaoController.Current.RemoveCustomer(_currentCustomer.No_);
        }

        public CustomerDetailViewModel(Customer customer)
        {
            Countries = DaoController.Current.GetCountries();
            Cities = DaoController.Current.GetPostCodeCities();
            CurrentCustomer = customer;
            Error = "";
        }
        public CustomerDetailViewModel()
        {
            Countries = DaoController.Current.GetCountries();
            Cities = DaoController.Current.GetPostCodeCities();
            CurrentCustomer = new Customer
            {
                NeedsVATInvoice = true,
                IsNewCustomer = true,
                CustomerPriceGroup = "RETAIL"
            };
            Error = "";
            _isNew = true;
        }


        public string SaveCustomer()
        {
            string errorText = string.Empty;
            if (String.IsNullOrEmpty(_currentCustomer.Name))
            {
                errorText= "შეიყვანეთ კლიენტის სახელი";
            }
            //else if (String.IsNullOrEmpty(_currentCustomer.Address))
            //{
            //    errorText = "შეიყვანეთ იურიდიული მისამართი";
            //}
            else if (String.IsNullOrEmpty(_currentCustomer.VATRegistrationNo_))
            {
                errorText = "შეიყვანეთ საიდენტიფიკაციო კოდი";
            }
            else if (_currentCustomer.VATRegistrationNo_.Length != 11 && _currentCustomer.VATRegistrationNo_.Length != 9)
            {
                errorText = "საიდენტიფიკაციო კოდის სიგრძე უნდა იყო 9 ან 11";
            }
            //else if (String.IsNullOrEmpty(_currentCustomer.ShipToAddress))
            //{
            //    errorText = "შეიყვანეთ მიწოდების მისამართი";
            //}
            //else if (String.IsNullOrEmpty(_currentCustomer.City))
            //{
            //    errorText = "შეიყვანეთ ქალაქი";
            //}
            else if (String.IsNullOrEmpty(_currentCustomer.Contact))
            {
                errorText = "შეიყვანეთ საკონტაქტო პირი";
            }
            //else if (String.IsNullOrEmpty(_currentCustomer.Country_RegionCode))
            //{
            //    errorText = "შეიყვანეთ ქვეყანა";
            //}
            //else if (String.IsNullOrEmpty(_currentCustomer.City))
            //{
            //    errorText = "შეიყვანეთ ქალაქი";
            //}
            else if ( String.IsNullOrEmpty(_currentCustomer.PhoneNo_) || _currentCustomer.PhoneNo_.Length < 8)
            {
                errorText = "ტელეფონის ნომრის სიგრძე ნაკლებია 8 ზე";
            }
            if (errorText == string.Empty)
            {
                try
                {
                    if(_isNew)
                        CustomersManager.Current.SaveCustomer(_currentCustomer);
                    else
                        CustomersManager.Current.UpdateCustomer(_currentCustomer);
                }
                catch (Exception ex)
                {
                    errorText = ex.Message;
                }
            }
            Error = errorText;
            return errorText;
        }
    }
}
