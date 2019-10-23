using Aprimo.Epi.Extensions.API.Fields;
using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Assets
{
    public partial class Embedded
    {
        public Embedded()
        {
            this.Fields = new Field();
        }

        [JsonProperty("preview")]
        public Preview Preview { get; set; }

        [JsonProperty("thumbnail")]
        public Preview Thumbnail { get; set; }

        [JsonProperty("masterfilelatestversion")]
        public MasterFileversionList MasterFileVersion { get; set; }

        [JsonProperty("fields")]
        public Field Fields { get; set; }
    }
}