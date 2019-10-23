using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions.API.Classifications
{
    public class Classification : AprimoModelBase
    {
        [JsonProperty("_links")]
        public ClassificationLinks Links { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("namePath")]
        public object NamePath { get; set; }

        [JsonProperty("labelPath")]
        public object LabelPath { get; set; }

        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        [JsonProperty("sortIndex")]
        public int SortIndex { get; set; }

        [JsonProperty("sortOrder")]
        public string SortOrder { get; set; }

        [JsonProperty("labels")]
        public List<ClassificationLabel> Labels { get; set; }

        [JsonProperty("registeredFields")]
        public List<object> RegisteredFields { get; set; }

        [JsonProperty("registeredFieldGroups")]
        public List<object> RegisteredFieldGroups { get; set; }

        [JsonProperty("isRoot")]
        public bool IsRoot { get; set; }
    }
}