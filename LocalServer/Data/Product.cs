namespace OpenHIoT.LocalServer.Data
{
    public class Product 
    {
        public uint Id { get; set; }    //  assigned by OpenHIot
        public uint VId { get; set; }   // vendor   
        public string Name { get; set; }
        public string? Desc { get; set; }
        public string? CName { get; set; }  //class name
        public string? Assm { get; set; }
 //       public bool? Hub { get; set; }
        public bool? Edge { get; set; }
  //      public long LaTime { get; set; }
    }

    public class Vendor 
    {
        public uint Id { get; set; }     //assigned by OpenHIot
        public string Name { get; set; }
//        public long LaTime { get; set; }
    }
}
