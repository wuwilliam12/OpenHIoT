using Newtonsoft.Json;
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
using UserControl = System.Windows.Controls.UserControl;

namespace OpenHIoT.Client.Pages.Channel.DataList
{
    /// <summary>
    /// Interaction logic for N1DSettingControl.xaml
    /// </summary>
    public partial class ItemSettingControl : UserControl
    {

        ListViewControl listViewControl;
        DataListItem lvItem;
        public ListViewControl ListViewControl
        {
            set { 
                listViewControl = value;
                lvItem = new DataListItem();
                lvItem.CopyFrom( listViewControl.SelectedItem );

                dataSourceCntl.Visibility = Visibility.Visible;
                dispSettingCb.Visibility = Visibility.Visible;

                DataContext = lvItem;
                segsCntl.Segments = lvItem.Segments;
            }
        }

        public ItemSettingControl()
        {
            InitializeComponent();
            dataSourceCntl.SelectionChanged += dataSourceCntl_SelectionChanged;
            dataSourceCntl.devSelCntl.Init(null);
        }

        private void dataSourceCntl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataSourceCntl.SelectedItem == null)
                dispSettingCb.ItemsSource = null;
            else
            {
                HeadRt h = (HeadRt)dataSourceCntl.SelectedItem;
                if(DataListItemTemplate.Template != null)
                    dispSettingCb.ItemsSource = DataListItemTemplate.Template.Where(x => x.Category == h.Name);
            }
        }

        private void dispSettingCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dispSettingCb.SelectedItem != null)
                lvItem.Segments = ((TemplateItem)dispSettingCb.SelectedItem).Segments;
        }
        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            listViewControl.ShowDisplay();
         //   ListViewControl.RestartRefreshTimer();
        }
        private void okBtn_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            listViewControl.ShowDisplay();
 //           if (dataSourceCntl.SelectedItem != null)
            {
                lvItem.Channel.Head = (HeadRtC)dataSourceCntl.SelectedItem;
                listViewControl.UpdateSelectedItemSetting(lvItem);
            }
        }

        private void dispCurveCb_Checked(object sender, RoutedEventArgs e)
        {
            if (lvItem.CurveSetting == null)
                lvItem.CurveSetting = new CurveSetting()
                {
                    Samples = 100,
                    ScaleTop = 10,
                    ScaleBot = 100,
                };
            curveBd.Visibility = Visibility.Visible;
        }

        private void dispCurveCb_Unchecked(object sender, RoutedEventArgs e)
        {
            curveBd.Visibility = Visibility.Collapsed;
        }

    }
}
