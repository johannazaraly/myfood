using Microsoft.Data.Entity;
using myfoodapp.Business;
using myfoodapp.Model;
using myfoodapp.ViewModel;
using myfoodapp.WebApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace myfoodapp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        //private HTTPServer webServer;

        public LoadingPage()
        {
            this.InitializeComponent();
            this.Loaded += AboutPage_Loaded;
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            txtInfo.Text = "Database creation";

            using (var db = new LocalDataContext())
            {
                db.Database.Migrate();
                LocalDataContextExtension.EnsureSeedData(db);
            }

            txtInfo.Text = "Log Init";

            var taskLogFile = Task.Run(async () => { await LogModel.GetInstance.InitFileFolder(); });
            taskLogFile.Wait();

            txtInfo.Text = "User Settings Init";

            var taskUserFile = Task.Run(async () => { await UserSettingsModel.GetInstance.InitFileFolder(); });
            taskUserFile.Wait();

            txtInfo.Text = "Background Service Init";

            var mesureBackgroundTask = MeasureBackgroundTask.GetInstance;
            mesureBackgroundTask.Run();

            App.TryShowNewWindow<MainPage>();
        }

    }
}
