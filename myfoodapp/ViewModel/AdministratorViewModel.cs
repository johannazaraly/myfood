using GalaSoft.MvvmLight.Messaging;
using myfoodapp.Business;
using myfoodapp.Business.BackgroundWorkers;
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
    public class AdministratorViewModel : BindableBase
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

        private string uptimeSystem = string.Empty;
        public string UptimeSystem
        {
            get { return uptimeSystem; }
            set
            {
                uptimeSystem = value;
                OnPropertyChanged("UptimeSystem");
            }
        }

        public NotifyTaskCompletion<ObservableCollection<Log>> Logs { get; private set; }
        public NotifyTaskCompletion<ObservableCollection<Service>> Services { get; private set; } 

        public AdministratorViewModel()
        {
            Logs = new NotifyTaskCompletion<ObservableCollection<Log>>(logModel.GetLogsAsync());
           // Services = new NotifyTaskCompletion<ObservableCollection<Service>>(serviceModel.GetServicesAsync());
            //UptimeSystem = ServicesManager.GetInstance.UptimeService.UptimeSystem;
        }

        public void OnBackClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }

        public void OnEraseHistoricalRecordsClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = true;
            Messenger.Default.Send(new CloseFlyoutMessage());
        }

        public void OnArchiveLogsClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = true;
            Messenger.Default.Send(new CloseFlyoutMessage());
        }

        public void OnResetAllSettingsClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = true;
            Messenger.Default.Send(new CloseFlyoutMessage());
        }

        public void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = false;
            Logs = new NotifyTaskCompletion<ObservableCollection<Log>>(logModel.GetLogsAsync());
            //Services = new NotifyTaskCompletion<ObservableCollection<Service>>(serviceModel.GetServicesAsync());
            //UptimeSystem = ServicesManager.GetInstance.UptimeService.UptimeSystem;
        }

        public void OnOpenPaneClicked(object sender, RoutedEventArgs args)
        {
           
        }

        public void CancelClicked(object sender, RoutedEventArgs args)
        {
            Messenger.Default.Send(new CloseFlyoutMessage());
        }

    }
}
