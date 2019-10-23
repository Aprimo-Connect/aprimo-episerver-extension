using Aprimo.Epi.Extensions.Models;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aprimo.Epi.Extensions.Controllers
{
    [TemplateDescriptor(AvailableWithoutTag = true, Inherited = true)]
    public class AprimoAssetController : PartialContentController<AprimoAssetBase>
    {
        private readonly UrlResolver urlResolver;

        public AprimoAssetController(UrlResolver urlResolver)
        {
            this.urlResolver = urlResolver;
        }

        public override ActionResult Index(AprimoAssetBase currentContent)
        {
            if (currentContent is AprimoImageFile aprimoImageFile)
            {
                TagBuilder builder = new TagBuilder("img");
                string url = aprimoImageFile.AprimoPreview;
                if (aprimoImageFile.BinaryData != null && aprimoImageFile.BinaryData.ID != null)
                    url = this.urlResolver.GetUrl(aprimoImageFile.ContentLink);
                builder.MergeAttribute("src", url);

                if (!string.IsNullOrEmpty(aprimoImageFile.Title))
                    builder.MergeAttribute("title", aprimoImageFile.Title);

                if (!string.IsNullOrEmpty(aprimoImageFile.Description))
                    builder.MergeAttribute("alt", aprimoImageFile.Description);
                return base.Content(builder.ToString(TagRenderMode.SelfClosing));
            }
            else if (currentContent is AprimoVideoFile aprimoVideoFile)
            {
                if (aprimoVideoFile.BinaryData != null && aprimoVideoFile.BinaryData.ID != null)
                {
                    TagBuilder video = new TagBuilder("video");
                    video.MergeAttribute("class", "aprimo-video");
                    video.MergeAttribute("controls", string.Empty);

                    if (!string.IsNullOrWhiteSpace(aprimoVideoFile.AprimoPreview))
                        video.Attributes.Add("poster", aprimoVideoFile.AprimoPreview);

                    TagBuilder source = new TagBuilder("source");
                    source.MergeAttribute("type", "video/mp4");

                    string url = aprimoVideoFile.AprimoPreview;
                    if (aprimoVideoFile.BinaryData != null && aprimoVideoFile.BinaryData.ID != null)
                        url = this.urlResolver.GetUrl(aprimoVideoFile.ContentLink);
                    source.MergeAttribute("src", url);

                    video.InnerHtml += source.ToString(TagRenderMode.SelfClosing);

                    return base.Content(video.ToString());
                }
            }
            TagBuilder link = new TagBuilder("a");
            link.MergeAttribute("href", this.urlResolver.GetUrl(currentContent.ContentLink));
            link.SetInnerText(currentContent.Name);
            return base.Content(link.ToString());
        }
    }
}