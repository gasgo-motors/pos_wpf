using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PosClient.Helpers;
using PosClient.ViewModels;
using PosClient.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLayer;
using PosClient.Views.Dialogs;

namespace PosClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            //Imagebackground.ImageSource = ImageFromBytes;
            SetUserControl(Login.Current);

            Title = VersionNumber;
        }

        public string VersionNumber
        {
            get
            {
                try
                {
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                    {
                       return "ვერსია :" + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                       // return "ვერსია : 1.2" ;
                    }
                    return "";
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }

        public void SetUserControl(UserControl c)
        {
            if (((IPosUserControl) c).UserControlType == UserControlTypes.Login)
            {
                gridHeader.Visibility = Visibility.Collapsed;
                Login.Current.password_box.Password = "";
                Login.Current.textBox.Text = "";
                Login.Current.CurrentModel.UpdateErrorMessage();
            }
            else
            {
                (DataContext as MainWindowViewModel).RaisePropertyChangedAfterLogin();
                gridHeader.Visibility = Visibility.Visible;

            }
            (DataContext as MainWindowViewModel).ShowHomeBtn = ((IPosUserControl)c).ShowHomeBtn;
            (DataContext as MainWindowViewModel).HeaderText = ((IPosUserControl)c).HeaderText;
            (DataContext as MainWindowViewModel).PrevHeaders = ((IPosUserControl)c).PrevHeaders;

            (DataContext as MainWindowViewModel).HomeBtnSize = ((IPosUserControl)c).HomeButtonSize;
            (DataContext as MainWindowViewModel).HomeIconSize = ((IPosUserControl)c).HomeIconSize;

            mainScroll.VerticalScrollBarVisibility = ((IPosUserControl)c).ScrollBarVis;
            GridContainer.Children.Clear();
            GridContainer.Children.Add(c);
        }

        private  async void  MenuParameters_Click(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["UserDetail"];
            var user = PosUsersManager.Current.GetUser(App.Current.User.UserName);
            dialog.DataContext = new PosUserDetailViewModel(user);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
        }

        public async void ShowCutomErrorDialog(string title, string errorMessage)
        {
            var dialog = (BaseMetroDialog)this.Resources["CutomErrorDialog"];

            dialog.DataContext = errorMessage;
            dialog.Title = title;
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
            
        }

        private void MenuSignOut_Click(object sender, RoutedEventArgs e)
        {
            SetUserControl(Login.Current);
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            if( App.Current.PosSetting != null)
                SetUserControl(Main.Current);
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SettingsManager.Current.LoadMotivationPicture();
            (DataContext as MainWindowViewModel).UpdateMotivationPicture();
        }

        public  void Messageerror(string errorText, string errorHeader)
        {
            System.Windows.Forms.MessageBox.Show(errorText, errorHeader,
                          System.Windows.Forms.MessageBoxButtons.OK,
                          System.Windows.Forms.MessageBoxIcon.Error);
        }

        public  bool Messagequestion(string message, string header)
        {
            var result = System.Windows.Forms.MessageBox.Show(message, header,
                  System.Windows.Forms.MessageBoxButtons.YesNo,
                  System.Windows.Forms.MessageBoxIcon.Question);
            return result == System.Windows.Forms.DialogResult.Yes;
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Messagequestion("გსურთ პროგრამის გათიშვა?", ""))
            {

            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
