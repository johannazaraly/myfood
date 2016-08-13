using GalaSoft.MvvmLight.Messaging;
using myfoodapp.Business;
using myfoodapp.Business.Clock;
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
    public class ClockManagementViewModel : BindableBase
    {
        private LogModel logModel = LogModel.GetInstance;
        private DatabaseModel databaseModel = DatabaseModel.GetInstance;

        private bool isConnected = false;
        public bool IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                OnPropertyChanged("IsConnected");
            }
        }

        private string currentDate = string.Empty;
        public string CurrentDate
        {
            get { return currentDate; }
            set
            {
                currentDate = value;
                OnPropertyChanged("CurrentDate");
            }
        }

        private DateTimeOffset setDate;
        public DateTimeOffset SetDate
        {
            get { return setDate; }
            set
            {
                setDate = value;
                OnPropertyChanged("SetDate");
            }
        }

        private TimeSpan setTime;
        public TimeSpan SetTime
        {
            get { return setTime; }
            set
            {
                setTime = value;
                OnPropertyChanged("SetTime");
            }
        }

        public ClockManagementViewModel()
        {
            DateTime currentDate = DateTime.Now ;

            var task = Task.Run(async () => {
                currentDate = await databaseModel.GetLastMesureDate();
            });
            task.Wait();

            CurrentDate = currentDate.ToString();

            SetDate = new DateTimeOffset(currentDate);
            //SetTime = new TimeSpan(currentDate.Hour, currentDate.Minute, currentDate.Second);
        }

        public void OnBackClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }

        public void OnSetDateClicked(object sender, RoutedEventArgs args)
        {
            var clockManager = ClockManager.GetInstance;

            try
            {
                 var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;
                 mesureBackgroundTask.Completed += MesureBackgroundTask_Completed;
                 mesureBackgroundTask.Stop();       
            }
            catch (Exception ex)
            {
                logModel.AppendLog(Log.CreateErrorLog("Exception on Stopping Measure Service", ex));
            }
        }

        private void MesureBackgroundTask_Completed(object sender, EventArgs e)
        {
            try
            {
                var clockManager = ClockManager.GetInstance;
                var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;

                if (clockManager != null)
                {
                    var taskClock = Task.Run(async () =>
                    {
                        await clockManager.Connect();
                    });
                    taskClock.Wait();

                    var newDateTime = new DateTime(SetDate.Year, SetDate.Month, SetDate.Day, SetTime.Hours, SetTime.Minutes, SetTime.Seconds);
                    clockManager.SetDate(newDateTime);
                    CurrentDate = clockManager.ReadDate().ToString();

                    clockManager.Dispose();

                    var taskDb = Task.Run(async () =>
                    {
                        await databaseModel.DeleteAllMesures();
                    });
                    taskDb.Wait();

                    mesureBackgroundTask.Run();
                }
            }
            catch (Exception ex)
            {
                logModel.AppendLog(Log.CreateErrorLog("Exception on Set Date", ex));
            }
        }

        public void OnConnectClockClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }

        public void OnResetClockClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }

    }
}
