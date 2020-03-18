using BusinessLayer;
using PosClient.Helpers;
using PosClient.ViewModels;
using System;
using System.Collections.Generic;
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

namespace PosClient.Views
{
    public abstract class LoginController : PosUserControl<Login, LoginViewModel>
    {

    }


    public partial class Login : LoginController
    {
        public override string HeaderText
        {
            get
            {
                return "ავტორიზაცია";
            }
        }

        public Login()
        {
            InitializeComponent();
        }

        public override void Refresh()
        {
            DataContext = new LoginViewModel();
            password_box.Password = "";
        }


        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Login;
            }
        }

        public override bool ShowHomeBtn
        {
            get
            {
                return false;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            CurrentModel.Password = password_box.Password;
            CurrentModel.UserName = textBox.Text;
            bool loadMainPanel;
            if (CurrentModel.AuthenticateUserNameAndPassword(out loadMainPanel))
            {
                if( loadMainPanel)
                    NavigateToControl(UserControlTypes.Main);
                else
                    NavigateToControl(UserControlTypes.Settings);
            }
        }

        private void password_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button_Click(null, null);
            }
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if( e.Key == Key.Enter)
            {
                button_Click(null, null);
            }
        }
    }
}
