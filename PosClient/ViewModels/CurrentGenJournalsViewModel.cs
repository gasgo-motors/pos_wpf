using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BusinessLayer;
using DataLayer;
using DataLayer.Extensions;
using PosClient.Helpers;

namespace PosClient.ViewModels
{
    public class CurrentGenJournalsViewModel : PosViewModel
    {

        
        public decimal? Summary { get; set; }

        private List<GenJouranlView> _journals;
        public List<GenJouranlView> Journals 
        {
            get { return _journals != null ?  _journals.Where(i => !i.IsHeaden).ToList() : null; }
            set
            {
                if (value != _journals)
                {
                    _journals = value;
                    RaisePropertyChanged(() => Journals);
                }
            }
        }

        public bool InEditMode { get; set; }

        public Visibility EditBtnVisibility
        {
            get
            {
                return App.Current.User.UserType == PosUserTypes.Manager && !InEditMode
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public Visibility SaveCancelVisibility
        {
            get { return InEditMode ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility NonEditVisibility
        {
            get { return !InEditMode ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void UpdateEditiVisibilities()
        {
            RaisePropertyChanged(() => EditBtnVisibility);
            RaisePropertyChanged(() => SaveCancelVisibility);
            RaisePropertyChanged(() => NonEditVisibility);
        }

        public void SaveDates()
        {
            foreach (var j in _journals)
            {
                if (j.IsHeaden)
                {
                    DaoController.Current.deletejournal(j.LineNo, j.DocumentNo);
                }
                else if (j.PostingDate != j.PostingDateReal)
                {
                    if (j.PostingDate != j.PostingDateReal)
                    {
                        DaoController.Current.updateGenJournalPostingDate(j.LineNo, j.DocumentNo , j.PostingDate);
                    }
                }
            }

            Refresh();
        }

        public void DeleteRow(GenJouranlView row)
        {
            row.IsHeaden = true;
            RaisePropertyChanged(() => Journals);
            Summary = Journals.Sum(i => i.Amount);
            RaisePropertyChanged(() => Summary);
        }


        public void Refresh()
        {
            Journals = DaoController.Current.GetJournalLinesView();
            Summary = Journals.Sum(i => i.Amount);
            RaisePropertyChanged(() => Summary);
            InEditMode = false;
            UpdateEditiVisibilities();
        }
    }

}
