using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TreeGridView.Common
{
    public class TreeGridExpanderTb : System.Windows.Controls.Button
    {
        public static readonly DependencyProperty IsExpandedProperty = DependencyProperty
    .Register("IsExpanded", typeof(bool?), typeof(TreeGridExpanderTb) );

        public bool? IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
           /* get
            {
                
                if (Content == null)
                    return null;
                return (string)Content == " - ";
            }*/
            set
            {
                Content = value == null? null : (bool)value? "  -  " : "  +  ";
                SetValue(IsExpandedProperty, value);
            }
        }

        public TreeGridExpanderTb()
        {
            Content = " + ";
            Background = new SolidColorBrush(Colors.Transparent);
            BorderThickness = new Thickness(0);
            Click += TreeGridExpanderTb_Click;
        }

        private void TreeGridExpanderTb_Click(object sender, RoutedEventArgs e)
        {
            if (IsExpanded != null)
                IsExpanded = !IsExpanded;
        }
    }
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the item has children, then show the checkbox, otherwise hide it
            return ((bool)value ? Visibility.Visible : Visibility.Hidden);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}