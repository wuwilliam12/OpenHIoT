using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHIoT.BleEdge.Product
{
    public enum SparkplugDataType
    {
        SP_DATA_TYPE_UNKNOWN = 0,
        SP_DATA_TYPE_INT8 = 1,
        SP_DATA_TYPE_INT16 = 2,
        SP_DATA_TYPE_INT32 = 3,
        SP_DATA_TYPE_INT64 = 4,
        SP_DATA_TYPE_UINT8 = 5,
        SP_DATA_TYPE_UINT16 = 6,
        SP_DATA_TYPE_UINT32 = 7,
        SP_DATA_TYPE_UINT64 = 8,
        SP_DATA_TYPE_FLOAT = 9,
        SP_DATA_TYPE_DOUBLE = 10,
        SP_DATA_TYPE_BOOLEAN = 11,
        SP_DATA_TYPE_STRING = 12,
        SP_DATA_TYPE_DATETIME = 13,
        SP_DATA_TYPE_TEXT = 14,

            // Additional Metric Types
        SP_DATA_TYPE_UUID = 15,
        SP_DATA_TYPE_DATASET = 16,
        SP_DATA_TYPE_BYTES = 17,
        SP_DATA_TYPE_FILE = 18,
        SP_DATA_TYPE_TEMPLATE = 19,

                // Additional PropertyValue Types
        SP_DATA_TYPE_PROPERTYSET = 20,
        SP_DATA_TYPE_PROPERTYSET_LIST = 21,	

                // Array Types
        SP_DATA_TYPE_INT8_ARRAY = 22,
        SP_DATA_TYPE_INT16_ARRAY = 23,
        SP_DATA_TYPE_INT32_ARRAY = 24,
        SP_DATA_TYPE_INT64_ARRAY = 25,
        SP_DATA_TYPE_UINT8_ARRAY = 26,
        SP_DATA_TYPE_UINT16_ARRAY = 27,
        SP_DATA_TYPE_UINT32_ARRAY = 28,
        SP_DATA_TYPE_UINT64_ARRAY = 29,
        SP_DATA_TYPE_FLOAT_ARRAY = 30,
        SP_DATA_TYPE_DOUBLE_ARRAY = 31,
        SP_DATA_TYPE_BOOLEAN_ARRAY = 32,
        SP_DATA_TYPE_STRING_ARRAY = 33,
        SP_DATA_TYPE_DATETIME_ARRAY = 34 };
    public partial class SparkplugValue 
    {
        delegate object? GetValueDelegate(string str);
        delegate string? GetValueStringDelegate(object ob, string? format);
        delegate object ReadValDelegate(byte[] dat, ushort rd_pos, ushort size);
        delegate void WriteValDelegate(object ob, byte[] dat, ushort wr_pos);

        object? val;
        public SparkplugDataType DataType { get; set; }


        GetValueStringDelegate? getValueString;
        GetValueDelegate? getValue;
        ReadValDelegate? readVal;
        WriteValDelegate? writeVal;
        public string? ValueStr
        {
            get
            {
                if (val == null || getValueString == null) return null;
                return getValueString(val, Format);
            }
            set
            {
                val = value == null || getValue == null ? null : getValue(value);
            }
        }

        public string? Format { get; set; }

        public object? ValueOb
        {
            get { return val; }
            set { val = value; }
        }

        public SparkplugValue(SparkplugDataType dt)
        {
            DataType = dt;
            switch(dt)
            {
                case SparkplugDataType.SP_DATA_TYPE_UINT8:
                    readVal = SparkplugValue.ReadValUInt8;
                    writeVal = SparkplugValue.WriteValUInt8;
                    getValue = SparkplugValue.GetValueUInt8;
                    getValueString = SparkplugValue.GetValueStringUInt8;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT8_ARRAY:
                    readVal = SparkplugValue.ReadValUInt8s;
                    writeVal = SparkplugValue.WriteValUInt8s;
                    getValue = SparkplugValue.GetValueUInt8s;
                    getValueString = SparkplugValue.GetValueStringUInt8s;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT16:
                    readVal = SparkplugValue.ReadValUInt16;
                    writeVal = SparkplugValue.WriteValUInt16;
                    getValue = SparkplugValue.GetValueUInt16;
                    getValueString = SparkplugValue.GetValueStringUInt16;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT16_ARRAY:
                    readVal = SparkplugValue.ReadValUInt16s;
                    writeVal = SparkplugValue.WriteValUInt16s;
                    getValue = SparkplugValue.GetValueUInt16s;
                    getValueString = SparkplugValue.GetValueStringUInt16s;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT32:
                    readVal = SparkplugValue.ReadValUInt32;
                    writeVal = SparkplugValue.WriteValUInt32;
                    getValue = SparkplugValue.GetValueUInt32;
                    getValueString = SparkplugValue.GetValueStringUInt32;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT32_ARRAY:
                    readVal = SparkplugValue.ReadValUInt32s;
                    writeVal = SparkplugValue.WriteValUInt32s;
                    getValue = SparkplugValue.GetValueUInt32s;
                    getValueString = SparkplugValue.GetValueStringUInt32s;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT64:
                    readVal = SparkplugValue.ReadValUInt64;
                    writeVal = SparkplugValue.WriteValUInt64;
                    getValue = SparkplugValue.GetValueUInt64;
                    getValueString = SparkplugValue.GetValueStringUInt64;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_UINT64_ARRAY:
                    readVal = SparkplugValue.ReadValUInt64s;
                    writeVal = SparkplugValue.WriteValUInt64s;
                    getValue = SparkplugValue.GetValueUInt64s;
                    getValueString = SparkplugValue.GetValueStringUInt64s;
                    break;


                case SparkplugDataType.SP_DATA_TYPE_INT8:
                    readVal = SparkplugValue.ReadValInt8;
                    writeVal = SparkplugValue.WriteValInt8;
                    getValue = SparkplugValue.GetValueInt8;
                    getValueString = SparkplugValue.GetValueStringInt8;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_INT8_ARRAY:
                    readVal = SparkplugValue.ReadValInt8s;
                    writeVal = SparkplugValue.WriteValInt8s;
                    getValue = SparkplugValue.GetValueInt8s;
                    getValueString = SparkplugValue.GetValueStringInt8s;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_INT16:
                    readVal = SparkplugValue.ReadValInt16;
                    writeVal = SparkplugValue.WriteValInt16;
                    getValue = SparkplugValue.GetValueInt16;
                    getValueString = SparkplugValue.GetValueStringInt16;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_INT16_ARRAY:
                    readVal = SparkplugValue.ReadValInt16s;
                    writeVal = SparkplugValue.WriteValInt16s;
                    getValue = SparkplugValue.GetValueInt16s;
                    getValueString = SparkplugValue.GetValueStringInt16s;
                    break;

                case SparkplugDataType.SP_DATA_TYPE_INT32:
                    readVal = SparkplugValue.ReadValInt32;
                    writeVal = SparkplugValue.WriteValInt32;
                    getValue = SparkplugValue.GetValueInt32;
                    getValueString = SparkplugValue.GetValueStringInt32;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_INT32_ARRAY:
                    readVal = SparkplugValue.ReadValInt32s;
                    writeVal = SparkplugValue.WriteValInt32s;
                    getValue = SparkplugValue.GetValueInt32s;
                    getValueString = SparkplugValue.GetValueStringInt32s;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_INT64:
                    readVal = SparkplugValue.ReadValInt64;
                    writeVal = SparkplugValue.WriteValInt64;
                    getValue = SparkplugValue.GetValueInt64;
                    getValueString = SparkplugValue.GetValueStringInt64;
                    break;
                case SparkplugDataType.SP_DATA_TYPE_INT64_ARRAY:
                    readVal = SparkplugValue.ReadValInt64s;
                    writeVal = SparkplugValue.WriteValInt64s;
                    getValue = SparkplugValue.GetValueInt64s;
                    getValueString = SparkplugValue.GetValueStringInt64s;
                    break;

                case SparkplugDataType.SP_DATA_TYPE_FLOAT:
                    readVal = SparkplugValue.ReadValFloat;
                    writeVal = SparkplugValue.WriteValFloat;
                    getValue = SparkplugValue.GetValueFloat;
                    getValueString = SparkplugValue.GetValueStringFloat;
                    Format = "f2";
                    break;
                case SparkplugDataType.SP_DATA_TYPE_FLOAT_ARRAY:
                    readVal = SparkplugValue.ReadValFloats;
                    writeVal = SparkplugValue.WriteValFloats;
                    getValue = SparkplugValue.GetValueFloats;
                    getValueString = SparkplugValue.GetValueStringFloats;
                    Format = "f2";
                    break;

                case SparkplugDataType.SP_DATA_TYPE_DOUBLE:
                    readVal = SparkplugValue.ReadValDouble;
                    writeVal = SparkplugValue.WriteValDouble;
                    getValue = SparkplugValue.GetValueDouble;
                    getValueString = SparkplugValue.GetValueStringDouble;
                    Format = "f2";
                    break;
                case SparkplugDataType.SP_DATA_TYPE_DOUBLE_ARRAY:
                    readVal = SparkplugValue.ReadValDoubles;
                    writeVal = SparkplugValue.WriteValDoubles;
                    getValue = SparkplugValue.GetValueDoubles;
                    getValueString = SparkplugValue.GetValueStringDoubles;
                    Format = "f2";
                    break;


            }
            //getValue = GetValueFloat;
        }


        public  int GetDataSize ()
        {
            if (val == null) return 0;
            switch (DataType)
            {
                case SparkplugDataType.SP_DATA_TYPE_BOOLEAN:
                case SparkplugDataType.SP_DATA_TYPE_INT8:
                case SparkplugDataType.SP_DATA_TYPE_UINT8:
                    return 1;
                case SparkplugDataType.SP_DATA_TYPE_BOOLEAN_ARRAY:  
                case SparkplugDataType.SP_DATA_TYPE_INT8_ARRAY:
                case SparkplugDataType.SP_DATA_TYPE_UINT8_ARRAY:
                    return ((Array)val).Length + 2; 

                case SparkplugDataType.SP_DATA_TYPE_INT16:
                case SparkplugDataType.SP_DATA_TYPE_UINT16:
                    return 2;

                case SparkplugDataType.SP_DATA_TYPE_INT16_ARRAY:
                case SparkplugDataType.SP_DATA_TYPE_UINT16_ARRAY:
                    return 2 * ((Array)val).Length + 2;

                case SparkplugDataType.SP_DATA_TYPE_INT32:
                case SparkplugDataType.SP_DATA_TYPE_UINT32:
                case SparkplugDataType.SP_DATA_TYPE_FLOAT:
                    return 4;

                case SparkplugDataType.SP_DATA_TYPE_INT32_ARRAY:
                case SparkplugDataType.SP_DATA_TYPE_UINT32_ARRAY:
                case SparkplugDataType.SP_DATA_TYPE_FLOAT_ARRAY:
                    return 4 * ((Array)val).Length + 2;

                case SparkplugDataType.SP_DATA_TYPE_INT64:
                case SparkplugDataType.SP_DATA_TYPE_UINT64:
                case SparkplugDataType.SP_DATA_TYPE_DOUBLE:
                    return 8;

                case SparkplugDataType.SP_DATA_TYPE_INT64_ARRAY:
                case SparkplugDataType.SP_DATA_TYPE_UINT64_ARRAY:
                case SparkplugDataType.SP_DATA_TYPE_DOUBLE_ARRAY:
                    return 8 * ((Array)val).Length + 2;
            }
            return 0;
            //getValue = GetValueFloat;
        }

        public virtual void  ReadVal(byte[] dat, ushort rd_pos, ushort size)
        {
            if(readVal != null)
                val = readVal(dat, rd_pos, size);
        }

        public virtual void WriteVal(byte[] dat, ushort wr_pos)
        {
            if(writeVal != null && val != null)
                writeVal(val, dat, wr_pos);
        }



    }

}
