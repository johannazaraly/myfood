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
    public class SensorsMonitoringViewModel : BindableBase
    {
        private DatabaseModel databaseModel = new DatabaseModel();

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

        public NotifyTaskCompletion<List<Measure>> PHMeasures { get; private set; }
        public NotifyTaskCompletion<List<Measure>> WaterTemperatureMeasures { get; private set; }

        public SensorsMonitoringViewModel()
        {
            PHMeasures = new NotifyTaskCompletion<List<Measure>>(databaseModel.GetLastDayMesures(SensorTypeEnum.ph));
            WaterTemperatureMeasures = new NotifyTaskCompletion<List<Measure>>(databaseModel.GetLastDayMesures(SensorTypeEnum.waterTemperature));
        }

        public void OnBackClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }
       
        public void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = false;            
        }
    }
}
