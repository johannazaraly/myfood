using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using myfoodapp.ViewModel;
using myfoodapp.Business;
using GalaSoft.MvvmLight.Messaging;
using myfoodapp.Common;
using myfoodapp.WebApp;
using myfoodapp.Model;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

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
            this.ViewModel = new AquaponicsManagementViewModel();
            this.Loaded += MainPage_Loaded;
            this.InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = ViewModel;

            Messenger.Default.Register<RefreshDashboardMessage>(this, (mess) =>
            {
                App.TryShowNewWindow<MainPage>();
            });
        }

        public AquaponicsManagementViewModel ViewModel { get; set; }
    }
}
