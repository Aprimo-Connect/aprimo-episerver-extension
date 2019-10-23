using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using System.Configuration;
using System.Linq;

namespace Aprimo.Epi.Extensions.Services
{
    [ServiceConfiguration(ServiceType = typeof(IAprimoConnectorSettingsRepository), Lifecycle = ServiceInstanceScope.Singleton)]
    public class AprimoConnectorSettingsRepository : IAprimoConnectorSettingsRepository
    {
        protected DynamicDataStore GetStore()
        {
            return DynamicDataStoreFactory.Instance.GetStore(typeof(AprimoConnectorSettings)) ??
                  DynamicDataStoreFactory.Instance.CreateStore(typeof(AprimoConnectorSettings));
        }

        public AprimoConnectorSettings Load()
        {
            var store = GetStore();
            var settings = store.Items<AprimoConnectorSettings>().FirstOrDefault();
            if (settings == null)
            {
                settings = new AprimoConnectorSettings
                {
                    AprimoDAMUsername = ConfigurationManager.AppSettings["AprimoDAMUsername"],
                    AprimoDAMUserToken = ConfigurationManager.AppSettings["AprimoDAMUserToken"],
                    AprimoTenantId = ConfigurationManager.AppSettings["AprimoTenantId"],
                    AprimoDAMClientId = ConfigurationManager.AppSettings["AprimoDAMClientId"],
                    AprimoDAMARootClassificationId = ConfigurationManager.AppSettings["AprimoDAMARootClassificationId"],
                    AprimoDAMApiVersion = ConfigurationManager.AppSettings["AprimoDAMApiVersion"],
                    AprimoAssetPermalinkFormat = ConfigurationManager.AppSettings["AprimoAssetPermalinkFormat"],
                    InformationFields = ConfigurationManager.AppSettings["InformationFields"]
                };
                settings.Id = this.Save(settings);
            }
            return settings;
        }

        public Identity Save(AprimoConnectorSettings settings) =>
            this.GetStore().Save(settings);
    }
}