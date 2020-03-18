using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public partial class POSMessageEntry
    {
        public string CurrentUserId { get; set; }

        public string IsNewMessageFontWeight
        {
            get { return ReceiverID == CurrentUserId && !IsRead ? "Bold" : "Normal"; }
        }

        public bool IsMyLetter
        {
            get { return CurrentUserId == SenderID; }
        }

        public bool OnlyLocal
        {
            get { return string.IsNullOrEmpty(EntryNo); }
        }
        
    }
}
