using myfoodapp.Business;
using myfoodapp.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace myfoodapp.Model
{
    public class LogModel
    {
        private static readonly AsyncLock asyncLock = new AsyncLock();

        private static LogModel instance;

        public static LogModel GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LogModel();
                }
                return instance;
            }
        }

        private LogModel()
        {         
        }

        public async Task InitFileFolder()
        {
            var local = ApplicationData.Current.LocalFolder;
            try
            {
                var folder = await local.GetFolderAsync(@"LocalFiles\Logs\Current\");
                var files = await folder.GetFilesAsync();
                var currentFile = files.ToList().Where(t => t.Name == "logs.json").FirstOrDefault();
            }
            catch (FileNotFoundException ex)
            {
                var newFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(@"LocalFiles\Logs\Current\logs.json", CreationCollisionOption.FailIfExists);               
            }
        }

        public async Task<ObservableCollection<Log>> GetLogsAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(@"LocalFiles\Logs\Current\logs.json");

            if (file != null)
            {
                var read = await FileIO.ReadTextAsync(file);
                ObservableCollection<Log> logs = JsonConvert.DeserializeObject<ObservableCollection<Log>>(read);

                return logs;
            }

            return null;
        }

        public async void AppendLog(Log newLog)
        {
            using (await asyncLock.LockAsync())
            {
                var task = Task.Run(async () => { 
                
                var file = await ApplicationData.Current.LocalFolder.GetFileAsync(@"LocalFiles\Logs\Current\logs.json");

                if (file != null)
                {
                    var read = await FileIO.ReadTextAsync(file);
                    ObservableCollection<Log> currentLogs = JsonConvert.DeserializeObject<ObservableCollection<Log>>(read);

                    if (currentLogs == null)
                        currentLogs = new ObservableCollection<Log>();       

                    currentLogs.Add(newLog);

                    var str = JsonConvert.SerializeObject(currentLogs.OrderByDescending(l => l.date));

                    var newFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(@"LocalFiles\Logs\Current\logs.json", CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(newFile, str);

                }

                });

                task.Wait();
            }
        }
    }
}
