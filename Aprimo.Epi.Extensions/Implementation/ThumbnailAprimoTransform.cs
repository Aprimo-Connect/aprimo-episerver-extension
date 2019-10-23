using Aprimo.Epi.Extensions.Models;
using EPiServer;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Rest.ContentQuery;
using EPiServer.Cms.Shell.UI.Rest.Models;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms;
using EPiServer.Cms.Shell.UI.Rest.Models.Transforms.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Globalization;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions
{
    [ServiceConfiguration(typeof(IModelTransform), Lifecycle = ServiceInstanceScope.Transient)]
    public class ThumbnailAprimoTransform : StructureStoreModelTransform
    {
        private readonly UrlResolver urlResolver;

        private readonly TemplateResolver templateResolver;

        public ThumbnailAprimoTransform(IContentQueryHelper contentQueryHelper, IContentProviderManager contentProviderManager, ILanguageBranchRepository languageBranchRepository, IContentLanguageSettingsHandler contentLanguageSettingsHandler, ISiteDefinitionRepository siteDefinitionRepository, LanguageResolver languageResolver, IEnumerable<IHasChildrenEvaluator> hasChildrenEvaluator, IContentLoader contentLoader, TemplateResolver templateResolver, UrlResolver urlResolver) :
            base(contentProviderManager, languageBranchRepository, contentLanguageSettingsHandler, siteDefinitionRepository, hasChildrenEvaluator, languageResolver, urlResolver, templateResolver)
        {
            this.urlResolver = urlResolver;
            this.templateResolver = templateResolver;
        }

        public override void TransformInstance(IContent source, StructureStoreContentDataModel target, IModelTransformContext context)
        {
            base.TransformInstance(source, target, context);
            if (source is IAprimoImage)
            {
                UrlBuilder builder = new UrlBuilder(source.PreviewUrl(urlResolver, templateResolver));
                builder.Path = string.Format("{0}/Thumbnail", builder.Path);
                target.ThumbnailUrl = builder.ToString();
            }
        }
    }
}