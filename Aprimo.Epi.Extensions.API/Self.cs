using Newtonsoft.Json;
using System;

namespace Aprimo.Epi.Extensions.API
{
    public class Self
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }
    }
}