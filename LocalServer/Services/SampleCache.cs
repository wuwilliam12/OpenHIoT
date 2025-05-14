//using Newtonsoft.Json;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenHIoT.LocalServer.Services
{
    public class SampleDTO
    {
        public ulong Alias { get; set; }
  //      public uint HId { get; set; }
        public ulong TS { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? ValInt { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public double? ValDouble { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public byte[]? ValBlob { get; set; }

        public SampleDTO()
        {

        }

        public SampleDTO(Sample sample) {
            Alias = sample.Alias;
            TS = sample.TS;
            if (sample is SampleInt)
                ValInt = ((SampleInt)sample).Val;
            else if (sample is SampleReal)
                ValDouble = ((SampleReal)sample).Val;
            else if (sample is SampleBlob && ((SampleBlob)sample).Val != null)
          //      ValBlob = string.Concat(((SampleBlob)sample).Val.Select(b => b.ToString("X2")));
                ValBlob = ((SampleBlob)sample).Val;
            //    ValBlob = Encoding.UTF8.GetString(((SampleBlob)sample).Val);
        }
    }

    public class SampleDTOs : List<SampleDTO>
    {
        public SampleDTOs() { }
        public SampleDTOs(List<Sample> ss)
        {
            AddSamples(ss);
        }
       
        public void AddSamples(List<Sample> ss) 
        {
            foreach (var s in ss)
                Add(new SampleDTO(s));
        }
        public void AddSample(Sample s) 
        {
            Add(new SampleDTO(s));
        }

    }

    public interface ISampleCache
    {
        void Add(Sample v);
        SampleDTO? GetSampleByAlias(ulong alias);
        SampleDTOs? GetSamplesByAliasFrom(ulong alias, ulong from_ts);
        SampleDTOs GetSamplesByAlias(ulong[] ids);
        SampleDTOs GetSamplesByAliasFrom(ulong[] ids, ulong[] tss);
        SampleDTO? GetSampleByNsId(uint ns_id);
        SampleDTOs GetSamplesByNsIdFrom(uint ns_id, ulong from_ts);
        SampleDTOs GetSamplesByNsId(uint[] ns_ids);
        SampleDTOs GetSamplesByNsIdFrom(uint[] ns_ids, ulong from_ts);


        /*
                double? GetDouble(uint id);
                byte[]? GetBlob(uint id);
                double[] GetDoubles(uint[] ids);
                byte[][] GetBlobs(uint[] ids);
                double? GetDoubleByNs(uint id);
                byte[]? GetBlobByNs(uint ns_id);
                double[] GetDoublesByNs(uint[] ids);

                SampleInts? GetIntTs(uint id, ulong from_ts);
                SampleDoubles? GetDoublesTs(uint id, ulong from_ts);
                SampleBlobValues? GetBlobsTs(uint id, ulong from_ts);
        */


    }

    public class SampleCache : ISampleCache
    {
        ConcurrentDictionary<ulong, SampleItems> dict = new ConcurrentDictionary<ulong, SampleItems>();
        ConcurrentDictionary<uint, ulong?> dict_ns = new ConcurrentDictionary<uint, ulong?>();
        ulong buf_period = 60000;
        ulong max_none_read_period = 60000;
        IServiceScopeFactory scopeFactory;
        public SampleCache(IServiceScopeFactory _scopeFactory)
        {
            scopeFactory = _scopeFactory;
        }
        public void Add(Sample v)
        {
            if (dict.ContainsKey(v.Alias))
            {
                var buf = dict[v.Alias];
                if (v.TS > buf.LastReadTs && (v.TS - buf.LastReadTs) > max_none_read_period)
                    dict.Remove(v.Alias, out buf);
                else
                {
                    buf.Add(v);
                    while ( buf[0]==null || (v.TS - buf[0].TS) > buf_period)
                        buf.RemoveAt(0);
                }
            }
        }
        public SampleDTO? GetSampleByAlias(ulong id)
        {
            if (dict.ContainsKey(id))
            {
                SampleItems? buf;
                if (dict.TryGetValue(id, out buf))
                {
                    if (buf.Count > 0)
                    {
                        Sample s = buf.Last();
                        buf.LastReadTs = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();

                        if (s != null)
                            return new SampleDTO(s);
                    }
                }
            }
            else
                dict.TryAdd(id, new SampleItems());
            return null;
        }

        public SampleDTOs? GetSamplesByAliasFrom(ulong id, ulong from_ts)
        {
            if (dict.ContainsKey(id))
            {
                SampleItems? buf;
                if (dict.TryGetValue(id, out buf))
                {
                    buf.LastReadTs = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                    List<Sample> items = buf.Where(x => x.TS > from_ts).ToList();
                    if (items.Count == 0) return null;
                    return new SampleDTOs(items);
                }
            }
            else
                dict.TryAdd(id, new SampleItems());
            return null;
        }

        public SampleDTOs GetSamplesByAlias(ulong[] ids)
        {
            SampleDTOs ss = new SampleDTOs();
            foreach (ulong hid in ids )
            {
                SampleDTO? s = GetSampleByAlias(hid);
                if (s != null)
                    ss.Add(s);
            }
            return ss;
        }
        public SampleDTOs GetSamplesByAliasFrom(ulong[] ids, ulong[] tss)
        {
            SampleDTOs ss = new SampleDTOs();
            for(int i =0; i< ids.Length; i++)
            {
                SampleDTOs? s = GetSamplesByAliasFrom(ids[i], tss[i]);
                if (s != null)
                    ss.AddRange(s);
            }
            return ss;
        }
        public SampleDTO? GetSampleByNsId(uint ns_id)
        {
            if (dict_ns.ContainsKey(ns_id))
            {
                ulong? id = dict_ns[ns_id];
                if (id == null) return null;
                SampleItems? buf;
                if (dict.TryGetValue((ulong)id, out buf))
                {
                    if (buf.Count > 0)
                    {
                        buf.LastReadTs = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                        Sample s = buf.Last();
                        if (s != null)
                            return new SampleDTO(s);
                    }
                }
                else
                    dict.TryAdd((ulong)id, new SampleItems());
            }
            else
            {
                //dict_ns.TryAdd(ns_id, )
            }
            return null;
        }
        public SampleDTOs GetSamplesByNsId(uint[] ns_ids)
        {
            SampleDTOs ss = new SampleDTOs();
            foreach (uint ns_id in ns_ids)
            {
                SampleDTO? s = GetSampleByNsId(ns_id);
                if (s != null)
                    ss.Add(s);
            }
            return ss;
        }
        public SampleDTOs GetSamplesByNsIdFrom(uint ns_id, ulong from_ts)
        {
            if (dict_ns.ContainsKey(ns_id))
            {
                ulong? id = dict_ns[ns_id];
                if (id == null) return null;
                SampleItems? buf;
                if (dict.TryGetValue((ulong)id, out buf))
                {
                    buf.LastReadTs = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
                    List<Sample> items = buf.Where(x => x.TS > from_ts).ToList();
                    if (items.Count == 0) return null;
                    return new SampleDTOs(items);
                }
                else
                    dict.TryAdd((ulong)id, new SampleItems());
            }
            return new SampleDTOs();
        }


        public SampleDTOs GetSamplesByNsIdFrom(uint[] ns_ids, ulong from_ts)
        {
            SampleDTOs ss = new SampleDTOs();
            foreach (uint ns_id in ns_ids)
            {
                SampleDTOs? s = GetSamplesByNsIdFrom(ns_id, from_ts);
                if (s != null)
                    ss.AddRange(s);
            }
            return ss;
        }
  
    }
    public class SampleItems : List<Sample>
    {
        public ulong LastReadTs { get; set; }

        public SampleItems()
        {
            LastReadTs = (ulong)((DateTimeOffset)DateTime.Now).ToUnixTimeMilliseconds();
        }

    }

}