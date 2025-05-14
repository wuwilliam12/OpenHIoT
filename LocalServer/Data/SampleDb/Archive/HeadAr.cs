using Microsoft.EntityFrameworkCore;

namespace OpenHIoT.LocalServer.Data.SampleDb.Archive
{
    [Keyless]
    public class HeadAr
    {
        public string Name { get; set; }
        public string? Desc { get; set; }       //Description
        public uint? NsId { get; set; }         // NsID, alias, topic, deployed id
        public uint? SId { get; set; }          // Server Id 
        public ulong DId { get; set; }           //Device   
        public ushort? IId { get; set; }        //  internal id 
        public int? DType { get; set; }          //sparkplug Data Type 
        public string? UOM { get; set; }   //Unit of data
        public string? TOM { get; set; }   //Type of Unit
        public float? SA { get; set; }      //sampling rate - samples per second
                                            //     public int? Rec { get; set; }       //record
                                            //     public int? Del { get; set; }   //Deleted



    }
}
