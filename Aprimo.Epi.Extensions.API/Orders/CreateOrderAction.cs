using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Orders
{
    public class CreateOrderAction
    {
        [JsonProperty("action")]
        public string ActionAction { get; set; }

        [JsonProperty("parameters")]
        public CreateOrderParameters Parameters { get; set; }
    }
}