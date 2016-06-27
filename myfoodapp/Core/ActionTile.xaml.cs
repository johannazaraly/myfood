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
    public sealed partial class ActionTile : UserControl
    {
        public static readonly DependencyProperty ActionTitleProperty =
         DependencyProperty.Register("ActionTitle", typeof(string),
         typeof(ActionTile), new PropertyMetadata(null));

        public string ActionTitle
        {
            get { return (string)GetValue(ActionTitleProperty); }
            set { SetValue(ActionTitleProperty, value); }
        }

        public static readonly DependencyProperty ActionDescriptionProperty =
        DependencyProperty.Register("ActionDescription", typeof(string),
        typeof(ActionTile), new PropertyMetadata(null));

        public string ActionDescription
        {
            get { return (string)GetValue(ActionDescriptionProperty); }
            set { SetValue(ActionDescriptionProperty, value); }
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

        public ActionTile()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
    }
}
