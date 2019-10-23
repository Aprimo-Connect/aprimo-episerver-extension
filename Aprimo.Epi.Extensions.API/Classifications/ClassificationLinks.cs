using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Classifications
{
    public partial class ClassificationLinks
    {
        [JsonProperty("parent")]
        public Definition Parent { get; set; }

        [JsonProperty("ancestors")]
        public Definition Ancestors { get; set; }

        [JsonProperty("children")]
        public Definition Children { get; set; }

        [JsonProperty("image")]
        public Definition Image { get; set; }

        [JsonProperty("fields")]
        public Definition Fields { get; set; }

        [JsonProperty("recordpermissions")]
        public Definition Recordpermissions { get; set; }

        [JsonProperty("downloadpermissions")]
        public Definition Downloadpermissions { get; set; }

        [JsonProperty("classificationtreepermissions")]
        public Definition Classificationtreepermissions { get; set; }

        [JsonProperty("slaveclassifications")]
        public Definition Slaveclassifications { get; set; }

        [JsonProperty("classificationtreepermission")]
        public Definition Classificationtreepermission { get; set; }

        [JsonProperty("modifiedby")]
        public Definition Modifiedby { get; set; }

        [JsonProperty("createdby")]
        public Definition Createdby { get; set; }

        [JsonProperty("self")]
        public Self Self { get; set; }
    }
}