using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Assets
{
    public class AssetList : AprimoModelListBase<Asset>
    {
        [JsonProperty("_links")]
        public AssetItemLinks Links { get; set; }
    }
}