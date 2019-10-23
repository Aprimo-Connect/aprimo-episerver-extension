using Newtonsoft.Json;

namespace Aprimo.Epi.Extensions.API.Classifications
{
    public partial class ClassificationList : AprimoModelListBase<Classification>
    {
        public ClassificationList()
        {
            this.Links = new ClassificationLinks();
        }

        [JsonProperty("_links")]
        public ClassificationLinks Links { get; set; }
    }
}