using OpenHIoT.Client.Pages.Channel.Live;
using OpenHIoT.Client.Requests;
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


namespace OpenHIoT.Client.Pages.Channel
{
    /// <summary>
    /// Interaction logic for SettingControl.xaml
    /// </summary>
    public partial class SettingControl : System.Windows.Controls.UserControl
    {

        IDisplayLiveControl dispCntl;
        public IDisplayLiveControl DisplayControl
        {
            set {
                dispCntl = value;     
            }
        }
        
        public SettingControl()
        {
            InitializeComponent();
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            dispCntl.ShowDisplay();
        }
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            dispCntl.ShowDisplay();
        }
    }
}
