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
using Windows.ApplicationModel;
using Windows.Data.Json;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Xaml;

namespace myfoodapp.ViewModel
{
    public class AdvancedSettingsViewModel : BindableBase
    {
        private LogModel logModel = LogModel.GetInstance;
        private DatabaseModel databaseModel = DatabaseModel.GetInstance;

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

        private string packageVersion;
        public string PackageVersion
        {
            get { return packageVersion; }
            set
            {
                packageVersion = value;
                OnPropertyChanged("PackageVersion");
            }
        }

        private string freeDiskSpace;
        public string FreeDiskSpace
        {
            get { return freeDiskSpace; }
            set
            {
                freeDiskSpace = value;
                OnPropertyChanged("FreeDiskSpace");
            }
        }

        public AdvancedSettingsViewModel()
        {
            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            PackageVersion = String.Format("Release v.{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

            UInt64 intfreeDisk = 0;

            var taskFile = Task.Run(async () => { intfreeDisk = await GetFreeSpace(); });
            taskFile.Wait();

            FreeDiskSpace = String.Format("Free Disk Space: {0}Mo", intfreeDisk / 10000000);
        }

        public async Task<UInt64> GetFreeSpace()
        {
            StorageFolder local = ApplicationData.Current.LocalFolder;
            var retrivedProperties = await local.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" });
            return (UInt64)retrivedProperties["System.FreeSpace"];
        }

        public void OnBackClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<MainPage>();
        }

        public void OnEraseLogsClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = true;
            Messenger.Default.Send(new CloseFlyoutMessage());

            var task = Task.Run(async () => { await logModel.ClearLog(); });
            task.Wait();

            IsBusy = false;         
        }

        public void OnEraseMeasuresClicked(object sender, RoutedEventArgs args)
        {
            IsBusy = true;
            Messenger.Default.Send(new CloseFlyoutMessage());

            var task = Task.Run(async () => { await databaseModel.DeleteAllMesures(); });
            task.Wait();

            IsBusy = false;
        }

        public void OnRestartAppClicked(object sender, RoutedEventArgs args)
        {
            var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;
            mesureBackgroundTask.Completed += MesureBackgroundTask_Completed;
            mesureBackgroundTask.Stop();            
        }

        public void OnSaveClicked(object sender, RoutedEventArgs args)
        {
            var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;
            mesureBackgroundTask.Completed += SaveSettingMesureBackgroundTask_Completed;
            mesureBackgroundTask.Stop();
        }

        public void CancelClicked(object sender, RoutedEventArgs args)
        {
            Messenger.Default.Send(new CloseFlyoutMessage());
        }

        private void MesureBackgroundTask_Completed(object sender, EventArgs e)
        {
            var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;
            mesureBackgroundTask.Completed -= MesureBackgroundTask_Completed;

            try
            {
                Windows.ApplicationModel.Core.CoreApplication.Exit();
            }
            catch (Exception ex)
            {
                logModel.AppendLog(Log.CreateErrorLog("Exception on Restart App", ex));
            }
        }

        private void SaveSettingMesureBackgroundTask_Completed(object sender, EventArgs e)
        {
            var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;
            mesureBackgroundTask.Completed -= SaveSettingMesureBackgroundTask_Completed;

            try
            {
                //var taskDb = Task.Run(async () =>
                //    {
                //        await databaseModel.DeleteAllMesures();
                //    });
                //taskDb.Wait();

                mesureBackgroundTask.Run();               
            }
            catch (Exception ex)
            {
                logModel.AppendLog(Log.CreateErrorLog("Exception on Save Settings", ex));
            }
        }
    }
}
