using EPiServer.Shell;
using EPiServer.Shell.ViewComposition;

namespace Aprimo.Epi.Extensions.Providers
{
    [Component]
    public class AprimoDAMComponent : ComponentDefinitionBase
    {
        public AprimoDAMComponent()
            : base("epi-cms/component/Media")
        {
            this.Title = AprimoEpiConstants.ProviderName;
            this.Description = "Allows you to connect to Aprimo DAM for asset retrieval.";
            this.Categories = new string[] { "content" };
            this.PlugInAreas = new string[] { PlugInArea.AssetsDefaultGroup };
            this.Settings.Add(new Setting("repositoryKey", AprimoEpiConstants.ProviderKey));
            this.SortOrder = 900;
        }
    }
}