using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API
{
    public class ItemLink
    {
        [JsonProperty("self")]
        public Self Self { get; set; }

        [JsonProperty("definition")]
        public Definition Definition { get; set; }
    }
}