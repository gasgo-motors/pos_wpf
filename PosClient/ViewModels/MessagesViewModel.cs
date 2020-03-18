using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataLayer;
using PosClient.Helpers;
using PosClient.Views;

namespace PosClient.ViewModels
{
    public class MessagesViewModel : PosViewModel
    {

        private string _currentUserId;
        public string CurrentUserId
        {
            get { return _currentUserId; }
            set
            {
                if (_currentUserId != value)
                {
                    _currentUserId = value;
                    RaisePropertyChanged(() => CurrentUserId);
                }
            }
        }


        private List<string> _userSetupUserIds;
        public List<string> UserSetupUserIds
        {
            get { return _userSetupUserIds; }
            set
            {
                if (_userSetupUserIds != value)
                {
                    _userSetupUserIds = value;
                    RaisePropertyChanged(() => UserSetupUserIds);
                }
            }
        }


        private List<MessageUser> _messageUsers;
        public List<MessageUser> MessageUsers
        {
            get { return _messageUsers; }
            set
            {
                if (_messageUsers != value)
                {
                    _messageUsers = value;
                   RaisePropertyChanged(() => MessageUsers);
                }
            }
        }

        private string _filtereddUser;
        public string FiltereddUser
        {
            get { return _filtereddUser; }
            set
            {
                if (_filtereddUser != value)
                {
                    _filtereddUser = value;
                    RaisePropertyChanged(() => FiltereddUser);
                    if (MessageUsers != null)
                    {
                        foreach (var mu in MessageUsers)
                        {
                            mu.FilterUserId = _filtereddUser;
                        }
                        if (!string.IsNullOrEmpty(_filtereddUser) &&
                            MessageUsers.Count(i => i.UserFilterVisibility == Visibility.Visible) == 1)
                        {
                            var user = MessageUsers.First(i => i.UserFilterVisibility == Visibility.Visible);
                            SelectedUser = user.UserId;
                            MessageUsers.ForEach(i =>
                            {
                                i.IsSelected = false;
                            });
                            user.IsSelected = true;
                        }
                        Messages.Current.list_users.Items.Refresh();
                    }
                }
            }
        }



        private string _selectedUser;
        public string SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)
                {
                    _selectedUser = value;
                    RaisePropertyChanged(() => SelectedUser);
                    UpdateMessagesList(true);
                }
            }
        }

        private List<POSMessageEntry> _messageList;
        public List<POSMessageEntry> MessageList
        {
            get { return _messageList; }
            set
            {
                if (_messageList != value)
                {
                    _messageList = value;
                    RaisePropertyChanged(() => MessageList);
                }
            }
        }

        private string _messageText;
        public string MessageText
        {
            get { return _messageText; }
            set
            {
                if (_messageText != value)
                {
                    _messageText = value;
                    RaisePropertyChanged(() => MessageText);
                }
            }
        }



        public void Refresh()
        {
            CurrentUserId =  DaoController.Current.GetUserBySalesPersonCode(App.Current.PosSetting.Settings_SalesPersonCode);
            UserSetupUserIds = DaoController.Current.GetUserSetupIds(CurrentUserId);
            MessageUsers = DaoController.Current.GetUserMessagesCounts(CurrentUserId).Select(i => new MessageUser
            {
                UserId = i.Item1,
                SalesPersonCode = i.Item2,
                NewMessagesCount = i.Item3
            }).ToList();
            SelectedUser = null;
            MessageText = null;
            FiltereddUser = null;
        }




        public void SelectUser(MessageUser user)
        {
            SelectedUser = user.UserId;
            MessageUsers.ForEach(i =>
            {
                i.IsSelected = false;
            });
            user.IsSelected = true;
            Messages.Current.list_users.Items.Refresh();
        }

        public void DeleteMessage(POSMessageEntry message)
        {
            DaoController.Current.DeleteMessage(message.Id);
            MessageList.Remove(message);
            Messages.Current.grid_messages.Items.Refresh();
        }

        public void UpdateMessagesList(bool readUnreadMessages)
        {
            if (string.IsNullOrEmpty(CurrentUserId) || string.IsNullOrEmpty(SelectedUser))
            {
                MessageList = null;
            }
            else
            {
                MessageList = DaoController.Current.GetMessages(CurrentUserId, SelectedUser);
                if(readUnreadMessages)
                    DaoController.Current.UpdateReadStatus(CurrentUserId, SelectedUser);
            }
        }

        public void SendNewMessage()
        {
            try
            {
                if (!string.IsNullOrEmpty(CurrentUserId) && !string.IsNullOrEmpty(SelectedUser) &&
                    !string.IsNullOrEmpty(MessageText))
                {
                    var rsCode = MessageUsers.FirstOrDefault(i => i.UserId == SelectedUser).SalesPersonCode;
                    DaoController.Current.SendNewMessage(CurrentUserId, SelectedUser, App.Current.PosSetting.Settings_SalesPersonCode,
                        rsCode, MessageText);
                    MessageText = "";
                    UpdateMessagesList(false);
                }
            }
            catch (Exception ex)
            {
                App.Current.ShowErrorDialog("შეცდომა!", "ტექსტის ზომა არ უნდა აღემატებოდეს 250 სიმბოლოს!");
            }
        }
    }


    public class MessageUser
    {
        public string FilterUserId { get; set; }
        public string UserId { get; set; }
        public string SalesPersonCode { get; set; }
        public int NewMessagesCount { get; set; }

        public bool IsSelected { get; set; }

        public string BackGroundColor
        {
            get
            {
                return   IsSelected ? "#FFABDBF9" : "transparent";
            }
        }

        public Visibility NewMessageCountVisibility
        {
            get { return NewMessagesCount > 0 ? Visibility.Visible : Visibility.Collapsed; }
        }

        public Visibility UserFilterVisibility
        {
            get
            {
                return (string.IsNullOrEmpty(FilterUserId) || FilterUserId == UserId)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
    }
}
