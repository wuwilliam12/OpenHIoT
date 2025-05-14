using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
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

namespace OpenHIoT.Client.Pages.Channel.Live
{
    /// <summary>
    /// Interaction logic for DataSourceCntl.xaml
    /// </summary>
    public partial class DataSourceCntl : System.Windows.Controls.UserControl
    {
        public event SelectionChangedEventHandler SelectionChanged;

        public HeadRt? SelectedItem
        {
            get
            {
                if (mCb.SelectedItem == null) return null;
                return (HeadRt)mCb.SelectedItem;
            }
        }

        public DataSourceCntl()
        {
            InitializeComponent();
        }

        private void mCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(SelectionChanged != null)
                SelectionChanged(this, e);
        }

        private void devSelCntl_DeviceSelected(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems!= null && e.AddedItems.Count> 0)
            {
                LocalServer.Data.Device dev = (LocalServer.Data.Device)e.AddedItems[0];
                List<HeadRtC>? hs = SampletRtRequest.GetAllHeadsOfDevAsync(null, (ulong)dev.GetId()).Result;
                mCb.ItemsSource = hs;
            }
        }
    }
}
