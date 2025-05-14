using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.Client.Pages.Channel
{
    public enum DisplayType { DataList = 1, Chart = 2};
    internal class DisplaySetting
    {
        public List<DisplaySettingItem> Items { get; set; }
    }

    public class DisplaySettingItem
    {
        public string? Name { get; set; }
        public Size Size { get; set; }
        public DisplayType DType { get; set; }
        public uint RefreshInterval { get; set; }
        public string? Context { get; set; }
    }
}
