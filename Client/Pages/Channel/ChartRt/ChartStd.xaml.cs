using Newtonsoft.Json;
using OpenHIoT.Client.Pages.Channel.DataList;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Control = System.Windows.Controls.Control;
using UserControl = System.Windows.Controls.UserControl;

namespace OpenHIoT.Client.Pages.Channel.Live.ChartRt
{

    /// <summary>
    /// Interaction logic for ChartStd.xaml
    /// </summary>
    public partial class ChartStd : UserControl, IDisplayLiveControl
    {
        CurveSetting setting;
        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> ChartController { get; set; }

        public ChartStd()
        {
            InitializeComponent();
            ChartController = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
            ChartController.Range.MinimumY = 0;
            ChartController.Range.MaximumY = 1080;
            ChartController.Range.MaximumX = TimeSpan.FromSeconds(10);
            ChartController.Range.AutoY = true;
            ChartController.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Series 1",
                Stroke = Colors.Red,
            });
            DataContext = this;
            //   ChartController.PushData(2, 100);
        }

        public async Task UpdateDisplay()
        {

        }

        public DisplaySettingItem DisplaySettingItem {
            get
            {
                return new DisplaySettingItem()
                {
                    Name = Name,
                    DType = DisplayType.DataList,
                    Size = new System.Drawing.Size((int)Width, (int)Height),
                    Context = JsonConvert.SerializeObject(setting)
                };
            }
            set
            {

            }
        }


/*
        public Control GetSettingControl()
        {
            CurveSettingControl cntl =  new CurveSettingControl();
            return cntl;
        }
*/

        public void ShowDisplay()
        {
            throw new NotImplementedException();
        }
    }
}

