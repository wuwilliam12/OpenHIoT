using System.ComponentModel;

namespace OpenHIoT.LocalServer.Data;


public enum SyslogModule
{
    [Description("General")]
    General = 0,
    [Description("Sp Data")]
    MqttSpData = 1,
    [Description("Sp NData")]
    MqttSpNData,
    [Description("HiotMsg Data")]
    MqttHmMsgData,
    [Description("HiotMsg NData")]
    MqttHmMsgNData,

    [Description("API Product")]
    ApiProduct,
    [Description("API Device")]
    ApiDevice,


    [Description("API Sample Rt")]
    ApiSampleRt,
    [Description("API Sample Ar")]
    ApiSampleAr,

    [Description("API User")]
    ApiUser,
    [Description("API UNS")]
    ApiUNS
}

public class SyslogItem
{
    public int Id { get; set; }
    public long Time { get; set; }
    public uint? Color { get; set; }
    public int? Module { get; set; }
    public string Msg { get; set; }
}
