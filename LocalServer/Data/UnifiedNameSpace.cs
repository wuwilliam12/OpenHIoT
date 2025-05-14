using Microsoft.AspNetCore.Hosting.Server;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace OpenHIoT.LocalServer.Data
{

    public enum UnsType { Area = 1, Loc = 2, Building = 3, Floor = 4, Room = 5,
        Dat = 0x10 };
    

    public class UnifiedNameSpace 
    {
        public static readonly char sep_char = '/'; 
        public uint Id { get; set; }       //  Id                                     
        public string Name { get; set; }   // Name​
        public string? Desc { get; set; }   //Description
        public uint? Type { get; set; }       // area/loc/building/floor/room/dat​
        public uint? TId { get; set; }       // type id, area/loc Id        
        public ulong? NodeId { get; set; }     // Serve/Device/Measurement Id, 
        public uint? NodeType { get; set; }
        public uint? PId { get; set; }      //    parent                                        
  //      public long LaTime { get; set; }
        public int? SId { get; set; }       //  Server Id     

        public long? LocId { get; set; }      //location id 
        public uint? LocSId { get; set; }      //location server(when created) id 
    }


}
