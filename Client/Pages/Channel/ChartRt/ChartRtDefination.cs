using RealTimeGraphX.DataPoints;
using RealTimeGraphX.WPF;
using RealTimeGraphX;
using RealTimeGraphX.Renderers;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;


namespace OpenHIoT.Client.Pages.Channel.Live.ChartRt
{
    public class DataSeries : HeadRt
    {

        public string Name {  get; set; }
        public Color Stroke { get; set; }

        public DataSeries() { }
        public void Save() {
           // Name = s.Name;
            
        
        }



        public WpfGraphDataSeries GetWpfGraphDataSeries()
        {
            var s = new WpfGraphDataSeries();
            s.Name = Name;
            s.Stroke = Stroke;
            return s;
        }



    
    }



    public class ChartRtDefination
    {
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public double MaxX { get; set; }
        public bool AutoY { get; set; }
        public List<DataSeries> DataSeriesCollection { get; set; }

        public ChartRtDefination()
        {

        }

        public WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> GetWpfGraphController()
        {
            var c = new WpfGraphController<TimeSpanDataPoint, DoubleDataPoint>();
             c.Range.MinimumY = MinY;
             c.Range.MaximumY = MaxY;
             c.Range.MaximumX = TimeSpan.FromSeconds(MaxX);
             c.Range.AutoY = AutoY;
             c.Range.AutoYFallbackMode = GraphRangeAutoYFallBackMode.MinMax;
            foreach (DataSeries s in DataSeriesCollection)
                c.DataSeriesCollection.Add(s.GetWpfGraphDataSeries());
            return c;
        }
        public ChartRtDefination(WpfGraphController<TimeSpanDataPoint, DoubleDataPoint> c)
        {
            MinY = c.Range.MinimumY;
            MaxY = c.Range.MaximumY;
            MaxX = c.Range.MaximumX.Value.TotalSeconds;
            AutoY = c.Range.AutoY;
            DataSeriesCollection = new List<DataSeries>();
    //        foreach (var s in c.DataSeriesCollection)
    //            DataSeriesCollection.Add(new DataSeries(s));
        }
      /*
        public void Start()
        {

            Task.Factory.StartNew(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                //   var y = 100;
                while (true)
                {
                    var y = Cursor.Position.Y;
                    List<DoubleDataPoint> yy = new List<DoubleDataPoint>()
                    {
                        y,
                        y + 20,
                        y + 40,
                        y + 60,
                        y + 80,
                    };

                    var x = watch.Elapsed;

                    List<TimeSpanDataPoint> xx = new List<TimeSpanDataPoint>()
                    {
                        x,
                        x,
                        x,
                        x,
                        x
                    };
                    MultiController.PushData(xx, yy);
                    Thread.Sleep(30);
                }
            }, TaskCreationOptions.LongRunning);
        }
  
                private Color GetRandomColor()
                {
                    return Color.FromRgb((byte)r.Next(50, 255), (byte)r.Next(50, 255), (byte)r.Next(50, 255));
                }
        */
    }
}
