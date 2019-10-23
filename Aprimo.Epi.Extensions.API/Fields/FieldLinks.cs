using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Fields
{
    public partial class FieldLinks
    {
        [JsonProperty("self")]
        public Self Self { get; set; }

        [JsonProperty("definition")]
        public Definition Definition { get; set; }
    }
}