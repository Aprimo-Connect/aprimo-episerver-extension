using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Classifications
{
    public partial class ClassificationLabel
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("languageId")]
        public string LanguageId { get; set; }
    }
}