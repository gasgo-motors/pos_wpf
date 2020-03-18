using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class SettingsManager : PosManager<SettingsManager>
    {
        public Setting CurrentSetting { get; set; }

        public byte[] MotivationPicture { get; set; }

        public void SetMotivationPicture (byte[] picture)
        {
            MotivationPicture = picture;
        }

        public bool LoadSettings()
        {
            CurrentSetting = DaoController.Current.GetSettings();
            if (CurrentSetting != null)
            {
                NavDbController.Current.SetSetting(CurrentSetting);
                return true;
            }
            else
                return false;

        }

        public void LoadMotivationPicture()
        {
            SetMotivationPicture(DaoController.Current.GetMotivationPicture());
        }

        public Setting GetSettings()
        {
            return DaoController.Current.GetSettings();
        }

        public void UpdateSettings(Setting setting)
        {
            DaoController.Current.UpdateSettings(setting);
            LoadSettings();
        }
    }
}
