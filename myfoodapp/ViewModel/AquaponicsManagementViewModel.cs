using myfoodapp.Common;
using myfoodapp.Model;
using myfoodapp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace myfoodapp.ViewModel
{
    public class AquaponicsManagementViewModel : BindableBase
    {
        private DatabaseModel databaseModel = DatabaseModel.GetInstance;

        public NotifyTaskCompletion<Decimal> CurrentValue { get; private set; }
        public NotifyTaskCompletion<Decimal> LastDayValue { get; private set; }

        public AquaponicsManagementViewModel()
        {
           
        }

        public void OnPHSensorClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<PhSensorsMonitoringPage>();
        }

        public void OnHealthClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<HealthMonitoringPage>();
        }

        public void OnNetworkClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<NetworkPage>();
        }

    }
}
