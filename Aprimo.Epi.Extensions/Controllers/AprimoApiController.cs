using Aprimo.Epi.Extensions.API;
using Aprimo.Epi.Extensions.API.Assets;
using Aprimo.Epi.Extensions.Models;
using Aprimo.Epi.Extensions.Services;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Editor;
using EPiServer.Logging.Compatibility;
using EPiServer.Web;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Aprimo.Epi.Extensions.Controllers
{
    public class AprimoApiController : Controller
    {
        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IContentLoader contentLoader;

        private readonly IAprimoRestService aprimoRestService;

        private readonly IdentityMappingService identityMappingService;

        private readonly IAprimoConnectorSettingsRepository settingsRepository;

        public AprimoApiController(IContentLoader contentLoader, IAprimoConnectorSettingsRepository settingsRepository, IdentityMappingService identityMappingService, IAprimoRestService aprimoDamRepository)
        {
            this.contentLoader = contentLoader;
            this.aprimoRestService = aprimoDamRepository;
            this.identityMappingService = identityMappingService;
            this.settingsRepository = settingsRepository;
        }

        public JsonResult ProcessData(ContentReference id)
        {
            var identity = this.identityMappingService.Get(id);
            var contentItem = this.contentLoader.Get<AprimoAssetBase>(identity.ContentLink);
            if (contentItem.BinaryData?.ID == null)
            {
                AprimoBlobHelper.DownloadMasterFile(contentItem);
            }
            Task.Run(() =>
            {
                var asset = this.aprimoRestService.GetAsset(contentItem.AprimoId, AprimoEpiConstants.SelectAssetFields);
                if (asset?.Embedded?.Fields != null)
                {
                    var fields = new Fields();
                    var permalinkField = asset.Embedded.Fields.Items.FirstOrDefault(x => x.FieldName.Equals(AprimoEpiConstants.AprimoEPiPermalinkFieldName));
                    if (permalinkField != null)
                    {
                        fields.Add(permalinkField.Id, $"{VirtualPathUtilityEx.RemoveTrailingSlash(SiteDefinition.Current.SiteUrl.OriginalString)}{PageEditing.GetEditUrl(contentItem.ContentLink)}");
                        this.aprimoRestService.AddOrUpdate(asset.Id, new AddUpdateRecordRequest(fields));
                    }
                }
            });
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public HttpStatusCodeResult UpdateDataItem(string id)
        {
            var settings = this.settingsRepository.Load();
            if (!string.IsNullOrWhiteSpace(settings.AprimoCallbackAuthorizationToken))
            {
                if (Request.Headers["aprimo-callback"] != null && Request.Headers["aprimo-callback"].ToString() == settings.AprimoCallbackAuthorizationToken)
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);
            }

            var recordId = new Guid(id).ToString("N");
            var identity = this.identityMappingService.List(AprimoEpiConstants.ProviderKey)
                .FirstOrDefault(x => x.ExternalIdentifier.AbsoluteUri.EndsWith(recordId, StringComparison.OrdinalIgnoreCase));
            var contentItem = this.contentLoader.Get<AprimoAssetBase>(identity.ContentLink);
            if (contentItem != null)
            {
                AprimoBlobHelper.DownloadThumbnail(contentItem, true);
                AprimoBlobHelper.DownloadMasterFile(contentItem, true);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.NotModified);
        }

    }
}