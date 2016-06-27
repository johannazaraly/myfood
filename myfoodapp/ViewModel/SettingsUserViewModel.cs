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

        public void OnMyConfigClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<GreenhouseConfigurationPage>();
        }

        public void OnAboutClicked(object sender, RoutedEventArgs args)
        {
            App.TryShowNewWindow<AboutPage>();
        }

        public void OnMyLocationClicked(object sender, RoutedEventArgs args)
        {
           
        }

        public void OnRegisterClicked(object sender, RoutedEventArgs args)
        {
            
        }

    }
}
