using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.SqlServer.Server;
using Newtonsoft;
using Newtonsoft.Json;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using Control = System.Windows.Controls.Control;

namespace OpenHIoT.Client.Pages.Channel.DataList
{
    /// <summary>
    /// Interaction logic for ListViewControl.xaml
    /// </summary>
    public partial class ListViewControl : System.Windows.Controls.UserControl, IDisplayLiveControl 
    {
        DataListSetting lvSetting;
        DataListItem? item_sel;

        IEnumerable<System.Linq.IGrouping<uint?, DataListItem>> itemsByServers;

        public DataListItem SelectedItem
        {
            get {
                if (item_sel == null) return new();
                return item_sel; 
            }
        }

        public DataListSetting ListViewSetting
        {
            get {  return lvSetting;  }
            set {
                lvSetting = value;
                DataContext = lvSetting;
            }
        }
        public ListViewControl()
        {
            InitializeComponent();
            ListViewSetting = new DataListSetting();
            
        }
 
        public async Task UpdateDisplay()
        {
            if(itemsByServers == null)
                itemsByServers = lvSetting.Items.GroupBy(x=> x.Channel.SId);

            foreach (var g in itemsByServers)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference. 
                ulong[] m_ids = g.Select( x => (ulong)x.Channel.Alias ).ToArray();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (m_ids.Length > 0)
                {
                    DataListItem[] items = g.ToArray();
                    SampleDTOs? ss = await SampletRtRequest.GetSamples(g.Key, m_ids);
                    if (ss != null)
                    {
                        foreach (SampleDTO dto in ss)
                        {
                            DataListItem? item = g.FirstOrDefault(x => x.Channel.Alias == dto.Alias);
                            if(item != null)
                                item.SetSample(dto);
                        }
                    }
                }
            }
        }

        public DisplaySettingItem DisplaySettingItem
        {
            get
            {
                return new DisplaySettingItem()
                {
                    Name = Name,
                    DType = DisplayType.DataList,
                    Size = new System.Drawing.Size((int)Width, (int)Height),
                    //    RefreshInterval = (uint)refreshTimer.Interval,
                    Context = JsonConvert.SerializeObject(lvSetting,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore })
                };           
            }
            set
            {
                InitControl(value);
            }
        }

        void InitControl(DisplaySettingItem ds)
        {
      //      refreshTimer.Stop();
            Width = ds.Size.Width;
            Height = ds.Size.Height;
      //      refreshTimer.Interval = ds.RefreshInterval == 0? 1000 : ds.RefreshInterval;
            DataListSetting? v = ds.Context == null? new DataListSetting() : JsonConvert.DeserializeObject<DataListSetting>(ds.Context);
            lvSetting = v == null? new DataListSetting() : v;

            DataContext = lvSetting;
            foreach (DataListItem item in lvSetting.Items)
                InitItem(item);

            itemsByServers = lvSetting.Items.GroupBy(x => x.Channel.SId);
            titleLabel.Content = ds.Name;
            itemList.ItemsSource = lvSetting.Items;
            //      StartRefreshTimer();
        }

        void InitItem(DataListItem item)
        {
            item.Channel.SetDataSource();
            if (item.Channel.Head != null)
            {
                if (item.Format != null)
                    item.Format = item.Channel.Head.Format;
                if (item.UOM != null)
                    item.UOM = $"({item.Channel.Head.UOM})";
                item.Options = item.Channel.Head.Options;
                
            }
            item.InitSetting();
        }

        public void ShowDisplay()
        {
            if (item_sel != null)
                InitItem(item_sel);
            mainGd.Visibility = Visibility.Visible;
            itemsByServers = lvSetting.Items.GroupBy(x => x.Channel.SId);
        }

        private void rangeSeg_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void addNewButton_Click(object sender, RoutedEventArgs e)
        {
            item_sel = null;
            ShowItemSettingCntl();
        }
        private void delButton_Click(object sender, RoutedEventArgs e)
        {
            if (itemList.SelectedItem is null)
                return;
            item_sel = (DataListItem)itemList.SelectedItem;
            itemList.ItemsSource = null;
            lvSetting.Items.Remove(item_sel);
            itemList.ItemsSource = lvSetting.Items;

            

        }
        private void itemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (itemList.SelectedItem is null)
                item_sel = null;
            else
            {
                item_sel = (DataListItem)itemList.SelectedItem;
                ShowItemSettingCntl( );
            }                        
        }

        public void ShowItemSettingCntl()
        {
      //      refreshTimer.Stop();
            ItemSettingControl itemCntl = new ItemSettingControl();
            itemCntl.VerticalAlignment = VerticalAlignment.Stretch;
            itemCntl.ListViewControl = this; 
            settingGd.Children.Clear();
            settingGd.Children.Add(itemCntl);
            mainGd.Visibility = Visibility.Collapsed;
        }

        public void UpdateSelectedItemSetting(DataListItem item)
        {
            if (item_sel == null) lvSetting.Items.Add(item);
            else item_sel.CopyFrom(item);
            DataContext = null;
            DataContext = lvSetting;
            itemList.ItemsSource = null;
            itemList.ItemsSource = lvSetting.Items;
            //      StartRefreshTimer();
        }


        public void ShowSettingCntl()
        {
            //      refreshTimer.Stop();
            SettingControl cntl = new SettingControl();
            cntl.DisplayControl = this;
            settingGd.Children.Clear();
            settingGd.Children.Add(cntl);
            mainGd.Visibility = Visibility.Collapsed;
        }

        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            if (itemList.SelectedItem == null)
                ShowSettingCntl();
            else
            {
                item_sel = (DataListItem)itemList.SelectedItem;
                ShowItemSettingCntl();
            }

        }

        private void titleLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            itemList.SelectedItem = null;
        }
    }
}


