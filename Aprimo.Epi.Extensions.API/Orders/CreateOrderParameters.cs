using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Orders
{
    public class CreateOrderParameters
    {
        public partial class Parameters
        {
            [JsonProperty("width")]
            public long Width { get; set; }

            [JsonProperty("height")]
            public long Height { get; set; }

            [JsonProperty("format")]
            public string Format { get; set; }

            [JsonProperty("resolution")]
            public long Resolution { get; set; }

            [JsonProperty("maxFileSize")]
            public long MaxFileSize { get; set; }

            [JsonProperty("colorSpace")]
            public string ColorSpace { get; set; }
        }
    }
}