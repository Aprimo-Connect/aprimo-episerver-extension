using Aprimo.Epi.Extensions.Implementation;
using Aprimo.Epi.Extensions.Services;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Aprimo.Epi.Extensions.Initialization
{
    [InitializableModule]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.ConfigurationComplete += (o, e) =>
            {
                //Register custom implementations that should be used in favour of the default implementations
                context.Services
                .AddSingleton<ContentMediaResolver, AprimoMediaDataResolver>()
                .AddTransient<IModelTransform, ThumbnailAprimoTransform>();
            };
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}