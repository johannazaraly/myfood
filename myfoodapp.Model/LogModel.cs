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
        private string FILE_NAME = "logs.json";
        private StorageFolder folder = ApplicationData.Current.LocalFolder;

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
            if (await folder.TryGetItemAsync(FILE_NAME) != null)
            {
                var task = Task.Run(async () => {
                    var file = await folder.GetFileAsync(FILE_NAME);
                    await file.RenameAsync(String.Format("{0}_{1}.json", file.Name, file.DateCreated.ToString("yyyyMMddHHmmss")));
                });
                task.Wait();         
            }

            var newFile = await folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
        }

        public async Task<List<Log>> GetLogsAsync()
        {
            using (await asyncLock.LockAsync())
            {
                var file = await folder.GetFileAsync(FILE_NAME);

                if (file != null)
                {
                    var read = await FileIO.ReadTextAsync(file);
                    List<Log> logs = JsonConvert.DeserializeObject<List<Log>>(read);

                    return logs.OrderByDescending(l => l.date).ToList();
                }

                return null;
            }
        }

        public async void AppendLog(Log newLog)
        {
            using (await asyncLock.LockAsync())
            {
                var task = Task.Run(async () => { 
                
                var file = await folder.GetFileAsync(FILE_NAME);

                if (file != null)
                {
                    var read = await FileIO.ReadTextAsync(file);
                    ObservableCollection<Log> currentLogs = JsonConvert.DeserializeObject<ObservableCollection<Log>>(read);

                    if (currentLogs == null)
                        currentLogs = new ObservableCollection<Log>();       

                    currentLogs.Add(newLog);

                    var str = JsonConvert.SerializeObject(currentLogs.OrderByDescending(l => l.date));

                    var newFile = await folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(newFile, str);
                }

                });

                task.Wait();
            }
        }

        public async Task ClearLog()
        {
            using (await asyncLock.LockAsync())
            {
                var task = Task.Run(async () => {

                    var file = await folder.GetFileAsync(FILE_NAME);

                    if (file != null)
                    {
                        var newFile = await folder.CreateFileAsync(FILE_NAME, CreationCollisionOption.ReplaceExisting);
                    }
                });

                task.Wait();
            }
        }
    }
}
