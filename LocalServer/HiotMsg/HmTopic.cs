using OpenHIoT.LocalServer.Data;
using OpenHIoT.LocalServer.Data;
using SparkplugNet.Core.Enumerations;
using SparkplugNet.Core.Extensions;
using SparkplugNet.Core.Topics;
using System.Security.Cryptography;

namespace OpenHIoT.LocalServer.HiotMsg
{
    public class HmTopic : Topic
    { 
        public static readonly string NameSpace1 = "hiotV1.0";
        public static readonly char sep_char = '/';
        /// <summary>
        /// The message types.
        /// </summary>
        private static readonly Dictionary<SparkplugMessageType, string> messageTypes =
            Enum.GetValues(typeof(SparkplugMessageType)).Cast<SparkplugMessageType>().ToDictionary(msg => msg, msg => msg.GetDescription());

        /// <summary>
        /// The messages types as string.
        /// </summary>
        private static readonly Dictionary<string, SparkplugMessageType> messageTypeFromString = messageTypes.ToDictionary(x => x.Value, x => x.Key);
        public HmTopic()
        {
            Ns = (int)TopicNamespace.hm1_0;
        }

        public static void Parse(string str, Topic stt)
        {
            string[] ss = str.Split(sep_char);
            if (ss.Length < 3) return ;
            if (ss[0] != NameSpace1) return ;
            stt.Ns = (int)TopicNamespace.hm1_0;

            stt.GId = ss[1];

            SparkplugMessageType mt; 
            if (messageTypeFromString.TryGetValue(ss[2], out mt))
                stt.MType = (int)mt;
            else
                return ;
  //          stt.EId = Convert.ToUInt64(ss[3]);
            if (ss.Length == 5)
                stt.DId = Convert.ToUInt64(ss[4]);
        }


    }
}
