using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions.API.Orders
{
    public partial class CreateOrderTarget
    {
        public CreateOrderTarget()
        {
            this.TargetTypes = new List<string>();
        }

        [JsonProperty("recordId")]
        public string RecordId { get; set; }

        [JsonProperty("targetTypes")]
        public List<string> TargetTypes { get; set; }

        [JsonProperty("assetType")]
        public string AssetType { get; set; }
    }
}