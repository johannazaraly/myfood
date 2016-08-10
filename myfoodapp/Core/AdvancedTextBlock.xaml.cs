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
    public sealed partial class AdvancedTextBlock : UserControl
    {
        public string TextBlockContent
        {
            get { return (string)GetValue(TextBlockContentPropertyProperty); }
            set { SetValue(TextBlockContentPropertyProperty, value); }
        }

        public static readonly DependencyProperty TextBlockContentPropertyProperty =
        DependencyProperty.Register(
           "TextBlockContentProperty",
           typeof(string),
           typeof(AdvancedTextBlock),
           new PropertyMetadata(0, new PropertyChangedCallback(OnTextChanged)));

        public AdvancedTextBlock()
        {
            this.InitializeComponent();
            (this.Content as FrameworkElement).DataContext = this;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AdvancedTextBlock cc = d as AdvancedTextBlock;
            string content = (string)e.NewValue;
            cc.TextBlockContent = content;
        }
    }
}
