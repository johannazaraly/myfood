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
using Windows.UI.Xaml.Controls;

namespace myfoodapp.ViewModel
{
    public class SensorsMonitoringViewModel : BindableBase
    {
        private DatabaseModel databaseModel = new DatabaseModel();
        private PivotItem currentPivotItem;

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

        public SensorsMonitoringViewModel()
        {
            
        }

        private void PHMeasures_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PHMeasures.PropertyChanged -= PHMeasures_PropertyChanged;
            IsBusy = false;
        }

        public void OnBackClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }

        public void OnSelectionChanged(object sender, RoutedEventArgs args)
        {
            var currentPivot = (Pivot)sender;
            currentPivotItem = (PivotItem)currentPivot.SelectedItem;

            GetMesures();
        }

        private void GetMesures()
        {
            IsBusy = true;

            if (currentPivotItem.Name == "lastDay")
                PHMeasures = new NotifyTaskCompletion<List<Measure>>(databaseModel.GetLastDayMesures(SensorTypeEnum.ph));

            if (currentPivotItem.Name == "lastWeek")
                PHMeasures = new NotifyTaskCompletion<List<Measure>>(databaseModel.GetLastWeeksMesures(SensorTypeEnum.ph));

            PHMeasures.PropertyChanged += PHMeasures_PropertyChanged;
        }

        public void OnRefreshClicked(object sender, RoutedEventArgs args)
        {
            GetMesures();
        }
    }
}
