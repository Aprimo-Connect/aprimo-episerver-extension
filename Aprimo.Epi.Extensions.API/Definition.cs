using Newtonsoft.Json;
using System;

namespace Aprimo.Epi.Extensions.API
{
    public partial class Definition
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("select-key")]
        public string SelectKey { get; set; }
    }
}