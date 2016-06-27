using myfoodapp.Business;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace myfoodapp.Model
{
    public class ServiceModel
    {
        public async Task<ObservableCollection<Service>> GetServicesAsync()
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync(@"LocalFiles\Services\services.json");

            if (file != null)
            {
                var read = await FileIO.ReadTextAsync(file);
                ObservableCollection<Service> services = JsonConvert.DeserializeObject<ObservableCollection<Service>>(read);

                return services;
            }

            return null;       
        }
    }
}
