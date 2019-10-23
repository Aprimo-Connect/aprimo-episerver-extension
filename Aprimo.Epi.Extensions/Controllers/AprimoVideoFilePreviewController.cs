﻿using Aprimo.Epi.Extensions.API;
using Aprimo.Epi.Extensions.Events;
using Aprimo.Epi.Extensions.Models;
using Aprimo.Epi.Extensions.Services;
using EPiServer.Cms.Shell;
using EPiServer.Cms.Shell.UI.Controllers.Preview.Internal;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Aprimo.Epi.Extensions.Controllers
{
    [TemplateDescriptor(Inherited = true, AvailableWithoutTag = false, TagString = RenderingTags.Edit, TemplateTypeCategory = TemplateTypeCategories.MvcController)]
    public class AprimoVideoFilePreviewController : MediaEditController<AprimoVideoFile>
    {
        private readonly TemplateResolver templateResolver;

        private readonly UrlResolver urlResolver;

        private readonly AprimoEventManager<IAprimoAsset> eventHandler;

        private readonly IAprimoConnectorSettingsRepository settingsRepository;

        private readonly AprimoConnectorSettings Settings;

        private readonly IContentTypeRepository contentTypeRepository;

        private readonly IAprimoRestService aprimoRestService;

        public AprimoVideoFilePreviewController(IAprimoRestService aprimoRESTService, IContentTypeRepository contentTypeRepository, IAprimoConnectorSettingsRepository settingsRepository, TemplateResolver templateResolver, UrlResolver urlResolver)
        {
            this.templateResolver = templateResolver;
            this.urlResolver = urlResolver;
            this.contentTypeRepository = contentTypeRepository;
            this.aprimoRestService = aprimoRESTService;
            this.eventHandler = AprimoEventManager<IAprimoAsset>.Instance;

            this.settingsRepository = settingsRepository;
            this.Settings = settingsRepository.Load();
        }

        protected override string GetPreviewContent(AprimoVideoFile content)
        {
            var args = new AprimoAssetEventArgs<IAprimoAsset>(content);
            this.eventHandler.TriggerOnPreviewingContent(this, args);

            var settings = this.settingsRepository.Load();
            string previewUrl = content.AprimoPreview;
            if (content.BinaryData?.ID != null)
                previewUrl = content.PreviewUrl(this.urlResolver, this.templateResolver);

            string aprimoUrl = string.Format(settings.AprimoAssetPermalinkFormat, settings.AprimoTenantId, content.AprimoId);
            StringBuilder sb = new StringBuilder("<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css\" integrity=\"sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T\" crossorigin=\"anonymous\">");
            sb.Append("<div class=\"container py-5\">");
            sb.Append("<div class=\"row\">");
            sb.Append("<div class=\"col-md-8\">");
            TagBuilder video = new TagBuilder("video");
            video.Attributes.Add("width", "100%");
            video.Attributes.Add("controls", string.Empty);

            if (!string.IsNullOrWhiteSpace(content.AprimoPreview))
                video.Attributes.Add("poster", content.AprimoPreview);

            TagBuilder source = new TagBuilder("source");
            source.Attributes.Add("src", content.PreviewUrl(this.urlResolver, this.templateResolver));
            source.Attributes.Add("type", "video/mp4");
            video.InnerHtml += source.ToString(TagRenderMode.SelfClosing);

            sb.Append(video.ToString());

            sb.Append("</div>");
            sb.Append("<div class=\"col-md-4\">");
            sb.AppendFormat("<p class=\"text-break\"><strong>Aprimo ID:</strong> {0}</p>", content.AprimoId);
            var contentType = this.contentTypeRepository.Load(content.ContentTypeID);
            foreach (var item in contentType.PropertyDefinitions.Where(x => x.DisplayEditUI))
            {
                var value = content.GetPropertyValue(item.Name);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    sb.AppendFormat("<p class=\"text-break\"><strong>{0}:</strong> {1}</p>", item.EditCaption, value);
                }
            }
            if (!string.IsNullOrWhiteSpace(settings.InformationFields))
            {
                var asset = this.aprimoRestService.GetAsset(content.AprimoId, AprimoEpiConstants.SelectAssetFields);
                var fieldsAndIds = settings.InformationFields.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                var fields = asset?.Embedded?.Fields?.Items;
                if (fields.Any())
                {
                    var mappedFields = fields.Where(x => fieldsAndIds.Contains(x.Id) || fieldsAndIds.Contains(x.FieldName));
                    if (mappedFields.Any())
                    {
                        foreach (var field in mappedFields)
                        {
                            if (field.LocalizedValues.Any())
                            {
                                var values = string.Join("<br/>", field.LocalizedValues.Select(x => x.Value));
                                sb.AppendFormat("<p class=\"text-break\"><strong>{0}:</strong> {1}</p>", field.Label ?? field.FieldName, values);
                            }
                        }
                    }
                }
            }

            sb.AppendFormat("<p class=\"text-break\"><strong>Asset Aprimo Url:</strong> <a href=\"{0}\" target=\"_blank\">View in Aprimo</a></p>", aprimoUrl);
            sb.Append("</div>");
            sb.Append("</div>");
            sb.Append("</div>");
            return sb.ToString();
        }
    }
}