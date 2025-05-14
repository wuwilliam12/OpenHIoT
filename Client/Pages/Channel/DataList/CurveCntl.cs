using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
using Windows.ApplicationModel.VoiceCommands;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Point = System.Windows.Point;

namespace OpenHIoT.Client.Pages.Channel.DataList
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class CurveCntl : System.Windows.Controls.Canvas   
    {
        public static readonly DependencyProperty SettingProperty =
            DependencyProperty.Register("Setting", typeof(CurveSetting), typeof(CurveCntl), new FrameworkPropertyMetadata(OnSettingChanged));
        public static readonly DependencyProperty SampleProperty =
            DependencyProperty.Register("Sample", typeof(SampleDTO), typeof(CurveCntl), new FrameworkPropertyMetadata(OnSampleChanged));
//        public static readonly DependencyProperty SegmentsProperty =
//            DependencyProperty.Register("Segments", typeof(ColorSegments), typeof(CurveCntl), new FrameworkPropertyMetadata(OnSegmentChanged));
         public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(System.Windows.Media.Color), typeof(CurveCntl), new FrameworkPropertyMetadata(OnColorChanged));


 //       System.Windows.Media.Color color;

        CurveSetting setting;
        SampleValues values;
        Polylines polylines;
        int totalSamples;
        double xScale;


        private static void OnSampleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurveCntl curveCntl = (CurveCntl)d;
            curveCntl.AddSample( (SampleDTO)e.NewValue );
        }
        private static void OnSettingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurveCntl curveCntl = (CurveCntl)d;
            curveCntl.Init((CurveSetting)e.NewValue);
        }
        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CurveCntl curveCntl = (CurveCntl)d;
            curveCntl.SetColor((System.Windows.Media.Color)e.NewValue);
        }

        public CurveSetting? Setting { get; set; }       
        public SampleDTO Sample  {   get; set ; }


        void AddSample(SampleDTO sample)
        {
            double? d = sample.ValDouble != null ? sample.ValDouble : sample.ValInt;
            if (d == null) return;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                SampleValue v = new SampleValue()
                {
                    TS = sample.TS,
                    Val = (double)d
                };

                values.Add(v);
                polylines.AddPoint(setting.GetYPos((double)d));
            }));

        }

        void Init(CurveSetting? _setting)
        {
            if (setting == null)
                Visibility = Visibility.Collapsed;
            else
            {
                setting = _setting;
                polylines.MaxSize = setting.Samples;

                totalSamples = setting.Samples;
                Visibility = Visibility.Visible;

            }          
        }

        void SetColor(System.Windows.Media.Color _color)
        {
            polylines.Color = _color;
        }

        public CurveCntl()
        {
            // InitializeComponent();
            setting = new CurveSetting();
            polylines = new Polylines(this);
            Background = System.Windows.Media.Brushes.White;
            values = new SampleValues();
            SizeChanged += CurveCntl_SizeChanged;
        }

        private void CurveCntl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setting.Height = e.NewSize.Height;

            xScale = (double)e.NewSize.Width / setting.Samples;
            polylines.ScaleX = xScale;
        }
    }
    public class CurveSetting
    {
        public int Samples { get; set; }
        public double ScaleTop { get; set; }
        public double ScaleBot { get; set; }

        double k, b;
        [JsonIgnore]
        public double Height
        {
            set
            {
                k = value / (ScaleBot - ScaleTop);
                b = - k * ScaleTop;
            }
        }

        public double GetYPos(double d)
        {
            return k * d + b;
        }
    }

    public class SampleValue
    {
        public double Val { get; set; }
        public ulong TS { get; set; }   
    }

    public class SampleValues : List<SampleValue>
    {

    }

    public class Polylines : List<Polyline>
    {
        Canvas canvas;
        int x;
        System.Windows.Media.Color color;
        public int MaxSize { get;  set;  }

        public double ScaleX { get; set; }  

        public System.Windows.Media.Color? Color
        {
            set
            {
                color = value == null ? System.Windows.Media.Colors.Black : (System.Windows.Media.Color)value; 
                Polyline p = new Polyline()
                {
                    Stroke = new System.Windows.Media.SolidColorBrush(color),
                    StrokeThickness = 1
                };
                canvas.Children.Add(p);
                Add(p);
            }
        }

        public Polylines(Canvas _canvas)
        {
            canvas = _canvas;
            Color = System.Windows.Media.Colors.Black; 
        }

        void LeftShiftPoints()
        {
            this[0].Points.RemoveAt(0);
            if (this[0].Points.Count == 0)
            {
                canvas.Children.Remove(this[0]);
                this.RemoveAt(0);
            }
            int k = 0;
            foreach(Polyline ps in this)
            {
                for(int i = 0; i < ps.Points.Count; i++)
                {
                    ps.Points[i] = new Point(k * ScaleX, ps.Points[i].Y);
                }
            }
        }

        public void AddPoint(double y)
        {
            if (MaxSize == 0) return;
            if (x < MaxSize)
                x++;
            else
                LeftShiftPoints();
               
            this.Last().Points.Add( new Point( x* ScaleX, y));
        }
    }
}
