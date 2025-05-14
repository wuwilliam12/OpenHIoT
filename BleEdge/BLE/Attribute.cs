using OpenHIoT.BleEdge.BLE;

namespace OpenHIoT.BleEdge.BLE
{
    public enum DiscoverType
    {
        BT_GATT_DISCOVER_PRIMARY, BT_GATT_DISCOVER_SECONDARY, BT_GATT_DISCOVER_INCLUDE, BT_GATT_DISCOVER_CHARACTERISTIC,
        BT_GATT_DISCOVER_DESCRIPTOR, BT_GATT_DISCOVER_ATTRIBUTE, BT_GATT_DISCOVER_STD_CHAR_DESC
    };

    [Flags]
    public enum GattChrcProperty
    {
        BT_GATT_CHRC_BROADCAST = 0x01,          //Characteristic Properties Bit field values.
        BT_GATT_CHRC_READ = 0x02,               //Characteristic read property.
        BT_GATT_CHRC_WRITE_WITHOUT_RESP = 0x04, //Characteristic write without response property.
        BT_GATT_CHRC_WRITE = 0x08,              //Characteristic write with response property.
        BT_GATT_CHRC_NOTIFY = 0x10,             //Characteristic notify property.
        BT_GATT_CHRC_INDICATE = 0x20,           //Characteristic indicate property.
        BT_GATT_CHRC_AUTH = 0x40,               //Characteristic Authenticated Signed Writes property.
        BT_GATT_CHRC_EXT_PROP = 0x80            //Characteristic Extended Properties property.
    };


    public class Attribute
    {
        public string Name { get; set; }
        public byte DiscoverType { get; set; }
        public byte Property { get; set; }
        public byte[] UUID { get; set; }

    }

    public class Attributes : List<Attribute>
    {
        //public DiscoverType DiscoverType { get; set; }
        //public GattChrcProperty Property { get; set; }

    }



    public class AttributeActive
    {
        public Attribute Base { get; set; }
        public ushort handle { get; set; }
        public BleEdge.Product.Device Device { get; set; }

    }

    public class AttributesActive : List<AttributeActive>
    {

    }
}