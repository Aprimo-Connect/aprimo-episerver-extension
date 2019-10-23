using Aprimo.Epi.Extensions.Providers;
using EPiServer;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;

namespace Aprimo.Epi.Extensions.Initialization
{
    [InitializableModule, ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class AprimoDAMInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // Create provider root if not exists
            var contentRepository = context.Locate.ContentRepository();
            var aprimoRoot = contentRepository.GetBySegment(SiteDefinition.Current.RootPage, AprimoEpiConstants.ProviderKey, LanguageSelector.AutoDetect(true));
            if (aprimoRoot == null)
            {
                aprimoRoot = contentRepository.GetDefault<ContentFolder>(SiteDefinition.Current.RootPage);
                aprimoRoot.Name = AprimoEpiConstants.ProviderName;
                contentRepository.Save(aprimoRoot, SaveAction.Publish, AccessLevel.NoAccess);
            }

            // Register the Aprimo content provider
            var contentProviderManager = context.Locate.Advanced.GetInstance<IContentProviderManager>();
            var configValues = new NameValueCollection { { ContentProviderElement.EntryPointString, aprimoRoot.ContentLink.ToString() } };
            var provider = context.Locate.Advanced.GetInstance<AprimoContentProvider>();
            provider.Initialize(AprimoEpiConstants.ProviderKey, configValues);
            contentProviderManager.ProviderMap.AddProvider(provider);

            // Since we have our structure outside asset root we register a custom route for it.
            RouteTable.Routes.MapContentRoute(
                name: "AprimoMedia",
                url: "{language}/aprimo/{node}/{partial}/{action}",
                defaults: new { action = "index" },
                contentRootResolver: (s) => aprimoRoot.ContentLink);

            // EPiServer UI needs the language segment
            RouteTable.Routes.MapContentRoute(
                name: "AprimoMediaEdit",
                url: CmsHomePath + "aprimo/{language}/{medianodeedit}/{partial}/{action}",
                defaults: new { action = "index" },
                contentRootResolver: (s) => aprimoRoot.ContentLink);
        }

        private static string CmsHomePath
        {
            get
            {
                var cmsContentPath = VirtualPathUtility.AppendTrailingSlash(EPiServer.Shell.Paths.ToResource("CMS", "Content"));
                return ServiceLocator.Current.GetInstance<IVirtualPathResolver>()
                    .ToAppRelative(cmsContentPath).Substring(1);
            }
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}