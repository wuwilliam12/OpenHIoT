using Newtonsoft.Json;
using OpenHIoT.Client.Pages.Channel.Live.ChartRt;
using OpenHIoT.Client.Pages.Channel.DataList;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Data.SampleDb.Archive;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft;
using Control = System.Windows.Controls.Control;
using System.IO;


namespace OpenHIoT.Client.Pages.Channel
{
    public interface IDisplayLiveControl
    {
        DisplaySettingItem DisplaySettingItem { get; set; }
        Task UpdateDisplay();
        void ShowDisplay();

    }

    /// <summary>
    /// Interaction logic for DisplayMainPage.xaml
    /// </summary>
    public partial class DisplayMainPage : System.Windows.Controls.Page
    {
        //   public static  List<MHeadRt>? mHeadsRt;
        //   IDisplayLiveControl? selectedCntl;

        System.Timers.Timer refreshTimer;
        public DisplayMainPage()
        {
            InitializeComponent();
            dtSelCb.ItemsSource = Enum.GetValues(typeof(DisplayType)).Cast<DisplayType>().ToList();
            LoadSetting();
            refreshTimer = new System.Timers.Timer();
            refreshTimer.Interval = 1000;
            refreshTimer.Elapsed += RefreshTimer_Elapsed;
            refreshTimer.Start();
        }

        private async void RefreshTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                foreach (Control cntl in displayWp.Children)
                    ((IDisplayLiveControl)cntl).UpdateDisplay();
            }));
        }

    

    void AddControlItem(DisplaySettingItem settingItem)
        {
            IDisplayLiveControl? new_cntl = null;
            switch (settingItem.DType)
            {
                case DisplayType.DataList:
                    new_cntl = new ListViewControl(); break;
                case DisplayType.Chart:
                    new_cntl = new ChartStd(); break;
            }
            if(new_cntl is not null)
            {
                displayWp.Children.Add((Control)new_cntl);
                new_cntl.DisplaySettingItem = settingItem;
            }
        }

        private void addNewButton_Click(object sender, RoutedEventArgs e)
        {
            addNewSp.Visibility = Visibility.Visible;
            addNewBtn.Visibility = Visibility.Collapsed;
            if (dtSelCb.SelectedItem == null)
                dtSelCb.SelectedItem = DisplayType.DataList;
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            addNewSp.Visibility = Visibility.Collapsed;
            addNewBtn.Visibility = Visibility.Visible;
   //         settingGd.Children.Clear();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            addNewSp.Visibility = Visibility.Collapsed;
            addNewBtn.Visibility = Visibility.Visible;
            if (dtSelCb.SelectedItem != null)
            {
                DisplayType dtb = (DisplayType)dtSelCb.SelectedItem;
                DisplaySettingItem? displaySettingItem = null;
                switch (dtb)
                {
                    case DisplayType.DataList:
                        displaySettingItem = new DisplaySettingItem()
                        {
                            Name = "DataList",
                            DType = DisplayType.DataList,
                            RefreshInterval = 1000,
                            Size = new System.Drawing.Size(300, 300),
                        };

                        break;
                    case DisplayType.Chart:
                        displaySettingItem = new DisplaySettingItem()
                        {
                            Name = "Chart",
                            DType = DisplayType.Chart,
                            RefreshInterval = 1000,
                            Size = new System.Drawing.Size(300, 300),
                        };
                        break;
                }
                if(displaySettingItem != null)
                    AddControlItem(displaySettingItem);
            }
        }

        private void dtSelCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        //    settingGd.Children.Clear();
      
           
        }

        private void mainGd_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.S:
                        SaveSetting(); break;
                    case Key.L:
                        LoadSetting(); break;
                }
            }
        }
        string setting_fn = "../../../dat/client/display_setting.json";
        void SaveSetting()
        {
            List<DisplaySettingItem> items = new List<DisplaySettingItem>();
            foreach (IDisplayLiveControl item in displayWp.Children)
                items.Add(item.DisplaySettingItem);

            string str = JsonConvert.SerializeObject(items, Formatting.Indented, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            using (FileStream fs = File.OpenWrite(setting_fn))
            {
                fs.SetLength(0);
                StreamWriter sw = new StreamWriter(fs);
                sw.Write(str);
                sw.Close();
            }
        }
        void LoadSetting()
        {
            try
            {
                using (FileStream fs = File.Open(setting_fn, FileMode.Open))
                {
                    StreamReader sr = new StreamReader(fs);
                    string str = sr.ReadToEnd();
                    sr.Close();

                    List<DisplaySettingItem>? items = JsonConvert.DeserializeObject<List<DisplaySettingItem>>(str);
                    if (items != null)
                    {
                        displayWp.Children.Clear();
                        foreach (DisplaySettingItem item in items)
                            AddControlItem(item);
                    }
                }
            }
            catch (Exception e)
            { }
        }

        private void mainGd_Loaded(object sender, RoutedEventArgs e)
        {
            mainGd.Focus();
        }
    }
}
