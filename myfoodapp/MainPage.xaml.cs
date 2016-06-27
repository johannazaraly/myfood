using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace myfoodapp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.UI.Xaml.DispatcherTimer aTimer = new Windows.UI.Xaml.DispatcherTimer();
            aTimer.Tick += ATimer_Tick;
            aTimer.Interval = new TimeSpan(50000000);
            //aTimer.Start();
        }

        private void ATimer_Tick(object sender, object e)
        {
            if (pivot.SelectedIndex < pivot.Items.Count - 1)
            {
                // If not at the last item, go to the next one.
                pivot.SelectedIndex += 1;
            }
            else
            {
                // The last PivotItem is selected, so loop around to the first item.
                pivot.SelectedIndex = 0;
            }
        }

    }

}
