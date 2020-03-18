using BusinessLayer;
using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosClient.ViewModels
{
    public class LoginViewModel : PosViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void UpdateErrorMessage()
        {
            ErrorMessage = "";
            RaisePropertyChanged(() => ErrorMessage);
        }

        public bool AuthenticateUserNameAndPassword(out bool loadMainPanel)
        {
            loadMainPanel = false;
            string errorMessage;
            if( !PosUsersManager.Current.Authenticate(UserName,Password, out errorMessage))
            {
                ErrorMessage = errorMessage;
                RaisePropertyChanged(() => ErrorMessage);
                return false;
            }
            loadMainPanel = SettingsManager.Current.LoadSettings();
            return true;
        }
    }
}
