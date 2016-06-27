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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace myfoodapp.Core
{
    public sealed partial class NotificationTile : UserControl
    {

        public static readonly DependencyProperty NotificationTitleProperty =
        DependencyProperty.Register("NotificationTitle", typeof(string),
        typeof(ActionTile), new PropertyMetadata(null));

        public string NotificationTitle
        {
            get { return (string)GetValue(NotificationTitleProperty); }
            set { SetValue(NotificationTitleProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(string),
        typeof(ActionTile), new PropertyMetadata(null));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueUnitProperty =
        DependencyProperty.Register("ValueUnit", typeof(string),
        typeof(ActionTile), new PropertyMetadata(null));

        public string ValueUnit
        {
            get { return (string)GetValue(ValueUnitProperty); }
            set { SetValue(ValueUnitProperty, value); }
        }

        public static readonly DependencyProperty LastValueProperty =
        DependencyProperty.Register("LastValue", typeof(string),
        typeof(ActionTile), new PropertyMetadata(null));

        public string LastValue
        {
            get { return (string)GetValue(LastValueProperty); }
            set { SetValue(LastValueProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
        DependencyProperty.Register("BackgroundColor", typeof(Brush),
        typeof(ActionTile), new PropertyMetadata(0));

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
        DependencyProperty.Register("ImagePath",
        typeof(string), typeof(ActionTile), new PropertyMetadata(null));

        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public NotificationTile()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
