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
using Xceed.Wpf.Toolkit;
using Color = System.Windows.Media.Color;
using UserControl = System.Windows.Controls.UserControl;

namespace OpenHIoT.Client.Pages.Channel.DataList
{
    /// <summary>
    /// Interaction logic for ColorSegmentsCntl.xaml
    /// </summary>
    public partial class ColorSegmentsCntl : UserControl
    {
        ColorSegments segments;

        public ColorSegments Segments
        {
            set
            { 
                segments = value;
                segLv.ItemsSource = segments;
            }
        }

        public ColorSegmentsCntl()
        {
            InitializeComponent();
        }

        private void addNewBtn_Click(object sender, RoutedEventArgs e)
        {
            segments.Add(new ColorSegment());
            segLv.ItemsSource = null;
            segLv.ItemsSource = segments;
        }

        private void ColorBtn_Click(object sender, RoutedEventArgs e)
        {
      
            SolidColorBrush b = (SolidColorBrush)((System.Windows.Controls.Button)sender).Background;
            ColorPicker colorPicker = new ColorPicker()
            {
                SelectedColor = b.Color
            };
          //  if(colorPicker.)


        }
    }

    public class ColorSegment
    {
        public string? Name { get; set; }
        public double From { get; set; }
        public double To { get; set; }
        public Color Color { get; set; }

        [JsonIgnore]
        public SolidColorBrush Brush
        {
            get
            {
                return new SolidColorBrush(Color);
            }
            set
            {
                Color = value.Color;
            }
        }


        public ColorSegment()
        {
            Color = System.Windows.Media.Colors.Black;
        }
        public ColorSegment(ColorSegment s)
        {
            Name = s.Name; From = s.From; To = s.To; Color = s.Color;
        }
/*
 * public void GetScaleX(double width, out double mul_x, out double offset_x)
        {
            mul_x = width / (To - From);
            offset_x = From * mul_x;
        }

        public System.Windows.Shapes.Rectangle GetRectangle()
        {
            //     Color c = Color == null ? Colors.Transparent :
            //         (Color)System.Windows.Media.ColorConverter.ConvertFromString(Color);
            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
            rect.Stroke = new SolidColorBrush(Color);
            //        rect.Fill = new SolidColorBrush(c);
            return rect;
        }
*/
    }

    public class ColorSegments : List<ColorSegment>
    {
        public Color? GetColor(double d)
        {
            ColorSegment? cg = this.Where(x => x.From <= d && x.To >= d).FirstOrDefault();
            if (cg == null) return null;
            return cg.Color;
        }


    }
}
