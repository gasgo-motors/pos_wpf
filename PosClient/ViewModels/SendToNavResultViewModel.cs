using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BusinessLayer;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class SendToNavResultViewModel : PosViewModel
    {
        public SendToNavResultViewModel() { }

        public SendToNavResultViewModel(int sCount, string errorNo, string errorM, List<RemainingItemEntry> rList)
        {
            SuccessCount = sCount;
            ErrorHeaderNo = errorNo;
            ErrorMessage = errorM;
            RemainingsList = rList;
        }
        public int SuccessCount { get; set; }

        public Visibility ShowSuccessMessage
        {
            get { return SuccessCount > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public string ErrorHeaderNo { get; set; }
        public string ErrorMessage { get; set; }

        public Visibility ShowErrorMessage
        {
            get { return  !string.IsNullOrEmpty(ErrorMessage) ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<RemainingItemEntry> RemainingsList { get; set; }

        public Visibility ShowRemainings
        {
            get { return RemainingsList != null && RemainingsList.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }
    }
}
