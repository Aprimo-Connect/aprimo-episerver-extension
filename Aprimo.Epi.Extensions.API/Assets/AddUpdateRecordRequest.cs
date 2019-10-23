using Aprimo.Epi.Extensions.API.Fields;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions.API.Assets
{
    public partial class AddUpdateRecordRequest
    {
        public AddUpdateRecordRequest()
        {
            this.Fields = new Fields();
        }

        public AddUpdateRecordRequest(Fields fields)
        {
            this.Fields = fields;
        }

        [JsonProperty("fields")]
        public Fields Fields { get; set; }
    }

    public partial class Fields
    {
        public Fields()
        {
            this.AddOrUpdate = new List<AddOrUpdateField>();
        }

        [JsonProperty("addOrUpdate")]
        public List<AddOrUpdateField> AddOrUpdate { get; set; }

        public void Add(AddOrUpdateField addOrUpdateField)
        {
            this.AddOrUpdate.Add(addOrUpdateField);
        }

        public void Add(string recordId, string value)
        {
            var fieldAddOrUpdate = new AddOrUpdateField(recordId);
            fieldAddOrUpdate.AddValue(value);
            this.Add(fieldAddOrUpdate);
        }
    }

    public partial class AddOrUpdateField
    {
        public AddOrUpdateField()
        {
            this.LocalizedValues = new List<LocalizedValue>();
        }

        public AddOrUpdateField(string recordId)
            : this()
        {
            this.Id = recordId;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("localizedValues")]
        public List<LocalizedValue> LocalizedValues { get; set; }

        public void AddValue(string value) =>
            this.AddValue("00000000000000000000000000000000", value);

        public void AddValue(string languageId, string value) =>
            this.LocalizedValues.Add(new LocalizedValue()
            {
                LanguageId = languageId,
                Value = value
            });
    }
}