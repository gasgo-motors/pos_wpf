using CoreTypes;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public abstract class PosManager<T> : Singleton<T> where T : new()
    {
        public PosUser User
        {
            get
            {
                return PosUsersManager.Current.CurrentUser;
            }
        }

        public Setting PosSetting
        {
            get
            {
                return SettingsManager.Current.CurrentSetting;
            }
        }


    }
}
