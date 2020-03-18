using MahApps.Metro;
using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BusinessLayer;

namespace PosClient.ViewModels
{
    public class MainWindowViewModel : PosViewModel
    {
        public MainWindowViewModel()
        {
            this.AccentColors = ThemeManager.Accents
                                .Select(a => new AccentColorMenuData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                .ToList();
        }

        public List<AccentColorMenuData> AccentColors { get; set; }

        private bool _showHomeBtn;
        public bool ShowHomeBtn
        {
            get { return _showHomeBtn; }
            set
            {
                _showHomeBtn = value;
                RaisePropertyChanged(() => ShowHomeBtn);
            }
        }

        private int _homeBtnSize;
        public int HomeBtnSize
        {
            get { return _homeBtnSize; }
            set
            {
                _homeBtnSize = value;
                RaisePropertyChanged(() => HomeBtnSize);
            }
        }

        private int _homeIconSize;
        public int HomeIconSize
        {
            get { return _homeIconSize; }
            set
            {
                _homeIconSize = value;
                RaisePropertyChanged(() => HomeIconSize);
            }
        }

        public void UpdateMotivationPicture()
        {
            RaisePropertyChanged(() => MotivationImage);
        }

        public BitmapImage MotivationImage
        {
            get
            {
                if (SettingsManager.Current.MotivationPicture == null)
                    return null;
                try
                {
                    using (var ms = new System.IO.MemoryStream(SettingsManager.Current.MotivationPicture))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        ms.Position = 0;
                        image.StreamSource = ms;
                        image.EndInit();
                        return image;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }


        private string _headerText;
        public string HeaderText
        {
            get { return _headerText; }
            set
            {
                _headerText = value;
                RaisePropertyChanged(() => HeaderText);
            }
        }

        private List<string> _prevHeaders;
        public List<string> PrevHeaders
        {
            get { return _prevHeaders; }
            set
            {
                _prevHeaders = value;
                RaisePropertyChanged(() => PrevHeaders);
            }
        }



        public string UserName
        {
            get
            {
                return App.Current.User != null ? App.Current.User.FullName : String.Empty;
            }
        }

        public void RaisePropertyChangedAfterLogin()
        {
            RaisePropertyChanged(() => UserName);
        }



    }
}
