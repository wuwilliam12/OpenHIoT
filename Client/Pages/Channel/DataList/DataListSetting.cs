using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft;

//using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel;
using SparkplugNet.VersionB.Data;
using System.Windows;
using OpenHIoT.Client.Requests;
using OpenHIoT.LocalServer.Services;
using System.Threading;
using Windows.ApplicationModel.VoiceCommands;
using Color = System.Windows.Media.Color;
namespace OpenHIoT.Client.Pages.Channel.DataList
{
    public class DataListSetting  
    {
        public double? Width {  get; set; }

        public float? FontSize {  get; set; }

        public List<DataListItem> Items { get; set; }

        public DataListSetting()
        {
            Items = new List<DataListItem>();
        }
    }


    public class DataListItem : INotifyPropertyChanged
    {
#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.

        bool setting_inited;
        Color color;
        public ChannelItem Channel { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Format { get; set; }

        string val;
        [JsonIgnore]
        public string Val
        {
            get { return val; }
        }

        public string? Options { get; set; }

        [JsonIgnore]
        public Color Color { get; set; }
        [JsonIgnore]
        public SolidColorBrush Brush
        {
            get
            {
                return new SolidColorBrush(color);
            }
            set
            {
                color = value.Color;
            }
        }
        public ColorSegments Segments { get; set; }

        public string? UOM { get; set; } 

        public bool? ShowCurve { get; set; }
        [JsonIgnore]
        public Visibility ShowVal
        {
            get
            {
                if (Options == null) return Visibility.Visible;
                return Visibility.Collapsed;
            }
        }

        public  CurveSetting CurveSetting { get; set; }

        [JsonIgnore]
        public SampleDTO Sample { get; set; }

        public DataListItem()
        {
            CurveSetting = new CurveSetting();  
            Channel = new ChannelItem();
            Segments = new();
            setting_inited = false;
            color = Colors.Black;
        }

        public void CopyFrom(DataListItem item)
        {
            if(item.Channel != null) 
                Channel = new ChannelItem(item.Channel);
   //         ShowSegments = item.ShowSegments;
            Segments.Clear();
            foreach (var v in item.Segments)
                Segments.Add(new ColorSegment(v));
            ShowCurve = item.ShowCurve;
            CurveSetting.Samples = item.CurveSetting.Samples;
            CurveSetting.ScaleTop = item.CurveSetting.ScaleTop;
            CurveSetting.ScaleBot = item.CurveSetting.ScaleBot;


        }

        public void InitSetting()
        {
            if ( PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("CurveSetting"));
                PropertyChanged(this, new PropertyChangedEventArgs("Segments"));
                PropertyChanged(this, new PropertyChangedEventArgs("Options"));
            }
        }

        public  void SetSample(SampleDTO sample)
        {
             double? d = null;
            if (sample.ValBlob != null)
                val = Encoding.UTF8.GetString(sample.ValBlob);
            else if (sample.ValInt != null)
            {
                long i = (long)sample.ValInt;
                if (Channel.Options != null && Channel.Options.Length > i && i > 0)
                    val = Channel.Options[i];
                else
                {
                    val = Format == null ? i.ToString() : i.ToString(Format);
                    d = i;
                }

            }
            else if (sample.ValDouble != null)
            {
                d = (double)sample.ValDouble;
                val = Format == null ? ((double)d).ToString("f2") : ((double)d).ToString(Format);
            }
            Sample = sample;
            if (PropertyChanged != null)
            {
                if(!setting_inited)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurveSetting"));
              //      PropertyChanged(this, new PropertyChangedEventArgs("Segments"));
                    setting_inited = true;
                }
                if (val != null )
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Val"));
                    PropertyChanged(this, new PropertyChangedEventArgs("Sample"));
                }
                if (d != null)
                {
                    Color? c = Segments.GetColor((double)d);
                    if (c != null) c = Colors.Black;
                    if(c != color)
                    {
                        c = color;
                        PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                    }
                }
            }
        }
    }

    public class TemplateItem
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string? Format { get; set; }
        public ColorSegments Segments { get; set; }

        public TemplateItem()
        {
            Segments = new ColorSegments();
        }
        public override string ToString()
        {
            return Name;
        }
    }


    public class DataListItemTemplate : List<TemplateItem>
    {
        static DataListItemTemplate? temps;
        public static DataListItemTemplate Template
        {
            get
            {
                if (temps == null)
                {
                    try
                    {
                        temps = JsonConvert.DeserializeObject<DataListItemTemplate>(DataListItemTemplate.str_temps);
                    }
                    catch (Exception ex) { }
                };
                return temps;
            }
        }                
        public static readonly string str_temps = @"[
            {
                'Category' : 'Temperature',
                'Name' : 'Room', 
                'DFormat' : 'f2', 
                Segments : [
                {   
                    'From' : 32,
                    'To' : 100,
                },{
                    'Name' : 'Cold',
                    'From' : 32,
                    'To' : 65,
                    'Color' : 'Blue'
                },{
                    'Name' : 'Hot',
                    'From' : 80,
                    'To' : 100,
                    'Color' : 'Red'
                }],
            },
            {
                'Category' : 'Humidity',
                'Name' : 'Room', 
                Segments : [
                {
                    'From' : 0,
                    'To' : 100,
                }, {
                    'Name' : 'Dry',
                    'From' : 0,
                    'To' : 30,
                    'Color' : 'Azure'
                },{
                    'Name' : 'Wet',
                    'From' : 60,
                    'To' : 100,
                    'Color' : 'LightBlue'
               } ]
            },
        ]";
    }



    

}
