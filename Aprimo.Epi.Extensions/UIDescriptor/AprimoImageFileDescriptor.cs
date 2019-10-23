using Aprimo.Epi.Extensions.Models;
using EPiServer.Shell;

namespace Aprimo.Epi.Extensions.UIDescriptor
{
    [UIDescriptorRegistration]
    public class AprimoImageFileDescriptor : UIDescriptor<AprimoImageFile>
    {
        public AprimoImageFileDescriptor()
            : base("epi-iconObjectImage")
        {
            DisabledViews = new string[] { };
            DefaultView = CmsViewNames.OnPageEditView;
        }
    }
}