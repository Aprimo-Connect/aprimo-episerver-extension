using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Aprimo.Epi.Extensions.Initialization
{
    [InitializableModule]
    public class AprimoCustomRoutesInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute(
                null,
                "aprimo/{controller}/{action}/{id}",
                new { controller = "aprimoapi", action = "Index" });
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}