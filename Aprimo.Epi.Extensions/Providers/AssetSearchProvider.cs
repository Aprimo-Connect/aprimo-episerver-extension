using Aprimo.Epi.Extensions.API;
using Aprimo.Epi.Extensions.API.Search;
using Aprimo.Epi.Extensions.Models;
using EPiServer;
using EPiServer.Cms.Shell.Search;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Localization;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using EPiServer.Shell.Search;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Epi.Extensions.Providers
{
    [SearchProvider]
    public class AssetSearchProvider : ContentSearchProviderBase<BasicContent, ContentType>
    {
        private readonly IAprimoRestService aprimoRestService;

        private readonly IdentityMappingService identityMappingService;

        private readonly IContentLoader contentLoader;

        public AssetSearchProvider(IAprimoRestService aprimoDataService, IContentLoader contentLoader, IdentityMappingService identityMappingService, LocalizationService localizationService, ISiteDefinitionResolver siteDefinitionResolver, IContentTypeRepository contentTypeRepository, EditUrlResolver editUrlResolver, ServiceAccessor<SiteDefinition> serviceAccessor, LanguageResolver languageResolver, UrlResolver urlResolver, TemplateResolver templateResolver, UIDescriptorRegistry uiDescriptorRegistry)
            : base(localizationService, siteDefinitionResolver, contentTypeRepository, editUrlResolver, serviceAccessor, languageResolver, urlResolver, templateResolver, uiDescriptorRegistry)
        {
            this.aprimoRestService = aprimoDataService;
            this.identityMappingService = identityMappingService;
            this.contentLoader = contentLoader;
        }

        public override IEnumerable<SearchResult> Search(Query query)
        {
            var searchResults = new List<SearchResult>();

            var assets = this.aprimoRestService.GetAssets(new SearchRequest()
            {
                LogRequest = true,
                SearchExpression = new SearchExpression()
                {
                    Expression = query.SearchQuery
                }
            }, AprimoEpiConstants.SelectAssetFields);
            if (assets != null)
            {
                var items = this.identityMappingService.List(AprimoEpiConstants.ProviderKey)
                    .ToList();

                foreach (var asset in assets.Items)
                {
                    var id = items.FirstOrDefault(x => x.ExternalIdentifier.ToString().EndsWith(asset.Id));
                    if (id != null)
                    {
                        var searchedAsset = this.contentLoader.Get<AprimoAssetBase>(id.ContentLink);
                        searchResults.Add(this.CreateSearchResult(searchedAsset));
                    }
                }
            }
            return searchResults;
        }

        public override string Area { get { return "aprimo"; } }

        public override string Category { get { return "aprimo"; } }

        protected override string IconCssClass { get { return ""; } }
    }
}