using GalaSoft.MvvmLight.Messaging;
using myfoodapp.Business;
using myfoodapp.Common;
using myfoodapp.Model;
using myfoodapp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace myfoodapp.ViewModel
{
    public class LogManagementViewModel : BindableBase
    {
        private LogModel logModel = LogModel.GetInstance;

        private bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged("IsBusy");
            }
        }

        public NotifyTaskCompletion<List<Log>> Logs { get; private set; }

        public LogManagementViewModel()
        {
            Logs = new NotifyTaskCompletion<List<Log>>(logModel.GetLogsAsync());
        }

        public void OnBackClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }
   
        public void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<LogManagementPage>();
        }

    }
}
