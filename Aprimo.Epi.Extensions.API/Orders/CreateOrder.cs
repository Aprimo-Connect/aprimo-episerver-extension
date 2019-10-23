using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions.API.Orders
{
    public class CreateOrder
    {
        public CreateOrder()
        {
            this.Targets = new List<CreateOrderTarget>();
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("creatorEmail")]
        public string CreatorEmail { get; set; }

        [JsonProperty("targets")]
        public List<CreateOrderTarget> Targets { get; set; }
    }
}