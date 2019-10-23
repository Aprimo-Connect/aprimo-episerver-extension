using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Assets
{
    public class Asset : AprimoModelBase
    {
        [JsonProperty("_links")]
        public AssetItemLinks Links { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("_embedded")]
        public Embedded Embedded { get; set; }
    }
}