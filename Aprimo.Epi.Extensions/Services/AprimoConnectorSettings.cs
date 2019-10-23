using EPiServer.Data;
using EPiServer.Data.Dynamic;

namespace Aprimo.Epi.Extensions.Services
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true, DatabaseDateTimeKind = EPiServer.Data.DatabaseDateTimeKind.Utc, StoreName = "AprimoConnectorSettings")]
    public class AprimoConnectorSettings : IDynamicData
    {
        public AprimoConnectorSettings()
        {
            this.Id = Identity.NewIdentity();
            this.AprimoDAMApiVersion = "1";
        }

        public Identity Id { get; set; }

        public string AprimoDAMUsername { get; set; }

        public string AprimoDAMClientId { get; set; }

        public string AprimoDAMUserToken { get; set; }

        public string AprimoTenantId { get; set; }

        public string AprimoDAMARootClassificationId { get; set; }

        public string AprimoDAMApiVersion { get; set; }

        public string AprimoAssetPermalinkFormat { get; set; }

        public string AprimoCallbackAuthorizationToken { get; set; }

        public string InformationFields { get; set; }

        public string AprimoOrderEmail { get; set; }
    }
}