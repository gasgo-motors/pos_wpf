using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataLayer;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class ItemCommentsDetailViewModel : PosViewModel
    {
        public ItemCommentsDetailViewModel()
        {
        }

        private string _itemNo;
        public ItemCommentsDetailViewModel(string itemNo)
        {
            _itemNo = itemNo;
            Refresh();
        }

        public Visibility CommentArrorVisibility { get; set; }

        public List<Comment_Line> Comments { get; set; }

        public string NewComment { get; set; }

        public void Refresh()
        {
            Comments = DaoController.Current.GetItemComments(_itemNo);
            NewComment = "";
            CommentArrorVisibility = Visibility.Collapsed;
            RaisePropertyChanged(() => Comments);
            RaisePropertyChanged(() => NewComment);
            RaisePropertyChanged(() => CommentArrorVisibility);
        }


        public void AddNewComment()
        {
            if (!string.IsNullOrEmpty(NewComment))
            {
                try
                {
                    DaoController.Current.AddNewItemComment(_itemNo, DateTime.Now, NewComment);
                    Refresh();
                }
                catch (Exception ex)
                {
                    CommentArrorVisibility = Visibility.Visible;
                    RaisePropertyChanged(() => CommentArrorVisibility);
                }
            }
        }

        public void RemoveComment(int lineNo)
        {
            DaoController.Current.RemoveItemComment(_itemNo, lineNo);
            Refresh();
        }
    }
}
