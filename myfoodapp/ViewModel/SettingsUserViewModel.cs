using myfoodapp.Common;
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
    public class SettingsViewModel : BindableBase
    {
        public SettingsViewModel()
        {
        }

        public void OnPHSensorSettingsClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<AboutPage>();
        }

        public void OnTempSensorSettingsClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<AboutPage>();
        }

        public void OnORPSensorSettingsClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<AboutPage>();
        }

        public void OnDOSensorSettingsClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<AboutPage>();
        }

    }
}
