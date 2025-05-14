using Microsoft.AspNetCore.Mvc;
using OpenHIoT.LocalServer.Data.SampleDb.Rt;
using OpenHIoT.LocalServer.Services;
using OpenHIoT.LocalServer.Data.Repository;

namespace OpenHIoT.LocalServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleRtController : ControllerBase
    {
     //   private readonly ILogger<MValueRtRepository> _logger;
        private readonly ISampleCache _mValueCache;
        //private readonly ISampleRtRepository _rep;
        public SampleRtController(ISampleRtRepository rep, ISampleCache mValueCache)
        {
           // _logger = logger;
          //  _rep = rep;
            _mValueCache = mValueCache;
        }
        [HttpGet]
        [Route("Sample/{id}")]
        public SampleDTO? GetSample(ulong id)
        {
            return _mValueCache.GetSampleByAlias(id);
        }

        [HttpGet]
        [Route("SampleByNs/{ns_id}")]
        public SampleDTO? GetSampleByNs(uint ns_id)
        {
            return _mValueCache.GetSampleByNsId(ns_id);
        }

        [HttpGet]
        [Route("Samples/{ids}")]
        public SampleDTOs GetSamples(string ids)
        {
            return _mValueCache.GetSamplesByAlias( ConvertToULongArray(ids) );
        }

        [HttpGet]
        [Route("SamplesByNs/{ns_ids}")]
        public SampleDTOs GetDoublesByNs(string ns_ids)
        {
            return _mValueCache.GetSamplesByNsId(ConvertToUintArray(ns_ids));
        }


        [HttpGet]
        [Route("SamplesFrom/{id}/{from_ts}")]
        public SampleDTOs? GetDoublesTs(ulong id, ulong from_ts)
        {
            return _mValueCache.GetSamplesByAliasFrom(id, from_ts);
        }

        [HttpGet]
        [Route("SamplesFromByNs/{ns_id}/{from_ts}")]
        public SampleDTOs? GetBlobsTs(uint ns_id, ulong from_ts)
        {
            return _mValueCache.GetSamplesByNsIdFrom(ns_id, from_ts);
        }

        [HttpGet]
        [Route("SamplesFromS/{ids}/{from_tss}")]
        public SampleDTOs? GetSamplesFrom(string ids, string from_tss)
        {
            ulong[] idss = ConvertToULongArray(ids);
            ulong[] tss = ConvertToULongArray(from_tss);
            return _mValueCache.GetSamplesByAliasFrom(idss, tss);
        }

        [HttpGet]
        [Route("SamplesFromByNsS/{ns_ids}/{from_ts}")]
        public SampleDTOs? GetSamplesFromByNs(string ns_ids, ulong from_ts)
        {
            return _mValueCache.GetSamplesByNsIdFrom(ConvertToUintArray(ns_ids), from_ts);
        }

        public static uint[] ConvertToUintArray(string str)
        {
            return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(uint.Parse)
                 .ToArray();
        }
        public static ulong[] ConvertToULongArray(string str)
        {
            return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                 .Select(ulong.Parse)
                 .ToArray();
        }
    }
}