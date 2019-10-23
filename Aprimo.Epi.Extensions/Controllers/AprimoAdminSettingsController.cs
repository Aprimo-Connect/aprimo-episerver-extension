using Aprimo.Epi.Extensions.Services;
using EPiServer.PlugIn;
using System.Web.Mvc;

namespace Aprimo.Epi.Extensions
{
    [EPiServer.PlugIn.GuiPlugIn(Area = PlugInArea.AdminMenu, UrlFromModuleFolder = "AprimoAdminSettings", DisplayName = "Aprimo Settings")]
    [Authorize(Roles = "CmsAdmins,Administrators,AprimoAdmins")]
    public class AprimoAdminSettingsController : Controller
    {
        private readonly IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository;

        public AprimoAdminSettingsController(IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository)
        {
            this.aprimoConnectorSettingsRepository = aprimoConnectorSettingsRepository;
        }

        public ActionResult Index()
        {
            var path = EPiServer.Shell.Paths.ToResource("Aprimo.Epi.Extensions", "1.0.0");
            var model = this.aprimoConnectorSettingsRepository.Load();
            return View($"{path}/Views/AprimoAdminSettings/index.cshtml", model);
        }

        [HttpPost]
        public ActionResult Index(AprimoConnectorSettings model)
        {
            var settings = this.aprimoConnectorSettingsRepository.Load();
            model.Id = settings.Id;
            this.aprimoConnectorSettingsRepository.Save(model);
            return RedirectToAction("Index");
        }
    }
}