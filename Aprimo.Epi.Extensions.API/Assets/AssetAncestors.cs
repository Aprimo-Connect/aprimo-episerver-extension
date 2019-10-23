using Newtonsoft.Json;
using System;

namespace Aprimo.Epi.Extensions.API.Assets
{
    public partial class AssetAncestors
    {
        [JsonProperty("href")]
        public Uri Href { get; set; }

        [JsonProperty("select-key")]
        public AssetSelectKey SelectKey { get; set; }
    }
}