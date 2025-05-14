namespace OpenHIoT.LocalServer.Data.SampleDb.Archive
{
    public class ValueBlock
    {
        public uint Id { get; set; }         // bin id
        public uint HId { get; set; }        // mhead id
        public uint? SId { get; set; }        // server id
        public long StartTime { get; set; }
        public long StopSTime { get; set; }

        public static int ElementSizeOf(SparkplugNet.VersionB.Data.DataType dt)
        {
            switch (dt)
            {
                case SparkplugNet.VersionB.Data.DataType.Boolean:
                case SparkplugNet.VersionB.Data.DataType.Int8:
                case SparkplugNet.VersionB.Data.DataType.UInt8: return 1;

                case SparkplugNet.VersionB.Data.DataType.BooleanArray:
                case SparkplugNet.VersionB.Data.DataType.Int8Array:
                case SparkplugNet.VersionB.Data.DataType.UInt8Array: return -1;

                case SparkplugNet.VersionB.Data.DataType.Int16:
                case SparkplugNet.VersionB.Data.DataType.UInt16: return 2;
                case SparkplugNet.VersionB.Data.DataType.Int16Array:
                case SparkplugNet.VersionB.Data.DataType.UInt16Array: return -2;

                case SparkplugNet.VersionB.Data.DataType.Int32:
                case SparkplugNet.VersionB.Data.DataType.UInt32:
                case SparkplugNet.VersionB.Data.DataType.Float: return 4;
                case SparkplugNet.VersionB.Data.DataType.Int32Array:
                case SparkplugNet.VersionB.Data.DataType.UInt32Array:
                case SparkplugNet.VersionB.Data.DataType.FloatArray: return -4;

                case SparkplugNet.VersionB.Data.DataType.Int64:
                case SparkplugNet.VersionB.Data.DataType.UInt64:
                case SparkplugNet.VersionB.Data.DataType.Double:
                case SparkplugNet.VersionB.Data.DataType.DateTime: return 8;
                case SparkplugNet.VersionB.Data.DataType.Int64Array:
                case SparkplugNet.VersionB.Data.DataType.UInt64Array:
                case SparkplugNet.VersionB.Data.DataType.DoubleArray:
                case SparkplugNet.VersionB.Data.DataType.DateTimeArray: return -8;

                //    case SparkplugNet.VersionB.Data.DataType.String: return 9;
                //     case SparkplugNet.VersionB.Data.DataType.Text: return 9;
                //     case SparkplugNet.VersionB.Data.DataType.StringArray: return -9;

                default:
                    return 0;

            }
        }
    }
}
