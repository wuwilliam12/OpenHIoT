using Microsoft.AspNetCore.Identity;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.ApplicationModel.UserDataTasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace OpenHIoT.Client.Pages.Channel.DataList
{
    /// <summary>
    /// Interaction logic for OptionsCntl.xaml
    /// </summary>
    public class OptionsRadios : System.Windows.Controls.StackPanel
    {
        public static readonly DependencyProperty OptionsProperty = DependencyProperty
    .Register("Options", typeof(string), typeof(OptionsRadios),  new FrameworkPropertyMetadata(SetOptions));
        public static readonly DependencyProperty ValProperty = DependencyProperty
    .Register("Val", typeof(double), typeof(OptionsRadios),  new FrameworkPropertyMetadata(SetVal));

        public string? Options { get; set; }
        public double Val { get; set; }  


        bool lock_update ;
        static int seq = 0;
        private static void SetVal(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OptionsRadios rads = (OptionsRadios)d;
            int v = (int)(double)e.NewValue;
            if (v < rads.Children.Count)
            {
                rads.lock_update = true;
             //   for(int i = 0; i < rads.Children.Count; i++)
                    ((System.Windows.Controls.RadioButton)rads.Children[v]).IsChecked = true;
                rads.lock_update = false;
            }
        }
            
        private static void SetOptions(DependencyObject d, DependencyPropertyChangedEventArgs e)      
        {
            OptionsRadios rads = (OptionsRadios)d;
            string? options = (string)e.NewValue;
            if (options == null)
            {
                rads.Visibility = Visibility.Collapsed;
                return;
            }
            string[] ss = options.Split(HeadRt.options_sep);
            string gn = $"G{seq}";
            for (int i = 0; i < ss.Length; i++)
            {

                System.Windows.Controls.RadioButton rb = new System.Windows.Controls.RadioButton()
                {
                    Content = ss[i],
                    GroupName = gn,
                    Margin = new Thickness(0,0,5,0)
                };
                rb.Checked += rads.Rb_Checked;
                rads.Children.Add(rb);
              
                // GroupName = "LanguageGroup" Command = "{Binding LanguageChangeCommand}" CommandParameter = "English" Content = "English" Style = "{StaticResource GroupToggleStyle}" FontSize = "30" Grid.Column = "3" Grid.Row = "3" BorderBrush = "#FF860707" />
            }  
            seq++;
        }

        private async void Rb_Checked(object sender, RoutedEventArgs e)
        {
            if (lock_update) return;
            DataListItem item  = (DataListItem)((System.Windows.Controls.RadioButton)sender).DataContext;
            if (item.Channel.Property == null || (item.Channel.Property & (int)ChannelProperty.Write) == 0)
                return;
            int k = 0;
            for(int i = 0; i < Children.Count; i++)
            {
                if(sender == Children[i])
                {
                    k = i; break;
                }
            }
            SampleDTO sampleDTO = new SampleDTO()
            {
                Alias = (ulong)item.Channel.Alias,
                ValInt = k,
            };
            await DeviceRequest.SetSampleValue(item.Channel.SId, sampleDTO);
        }

        public OptionsRadios()
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal;
            //   InitializeComponent();
            lock_update = false;    
        }
    }
}
