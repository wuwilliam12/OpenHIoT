using ProtoBuf;
using SparkplugNet.Core;
using SparkplugNet.Core.Interfaces;
using SparkplugNet.Core.Topics;
using SparkplugNet.VersionA.Data;
using SparkplugNet.VersionB.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Data.SQLite;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace OpenHIoT.LocalServer.Data;


public class SubscriptionItem
{
    public string CName { get; set; }
    public uint?  CId { get; set; }
    public string Topic { get; set; }

}