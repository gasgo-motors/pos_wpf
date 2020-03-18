using BusinessLayer;
using DataLayer;
using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using PosClient.Views;

namespace PosClient.ViewModels
{
    public class SettingsViewModel : PosViewModel
    {
        public Setting CurrentSettings { get; set; }


        private string _filterString;

        public string FilterString
        {
            get
            {
                return _filterString;
            }
            set
            {
                if (_filterString != value)
                {
                    _filterString = value;

                    PropertyInfo[] propertyInfos;
                    propertyInfos = typeof(Setting).GetProperties();
                    foreach (var p in propertyInfos)
                    {
                        var g = Settings.Current.FindName(p.Name) as Grid;
                        if(g == null) continue;
                        if (string.IsNullOrEmpty(_filterString) || p.Name.ToLower().Contains(_filterString.ToLower()) ||
                            p.Name.Replace("_", "").ToLower().Contains(_filterString.ToLower()))
                            g.Visibility = Visibility.Visible;
                        else
                            g.Visibility = Visibility.Collapsed;
                    }


                    RaisePropertyChanged(() => FilterString);
                }
            }
        }

        public SettingsViewModel()
        {
            CurrentSettings = new Setting();
        }

        public void Refresh()
        {
            CurrentSettings = SettingsManager.Current.GetSettings();
            RaisePropertyChanged(() => CurrentSettings);
        }

        public void Save()
        {
            SettingsManager.Current.UpdateSettings(CurrentSettings);

        }

        public void Cancel()
        {
            CurrentSettings = SettingsManager.Current.GetSettings();
            RaisePropertyChanged(() => CurrentSettings);
        }



        public void Import(Setting st)
        {
            CurrentSettings = st;
            RaisePropertyChanged(() => CurrentSettings);
        }

    }
}
