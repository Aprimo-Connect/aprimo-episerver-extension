using Aprimo.Epi.Extensions.Models;
using EPiServer.Shell;

namespace Aprimo.Epi.Extensions.UIDescriptor
{
    [UIDescriptorRegistration]
    public class AprimoVideoFileDescriptor : UIDescriptor<AprimoVideoFile>
    {
        public AprimoVideoFileDescriptor() : base("epi-iconObjectVideo")
        {
            DisabledViews = new string[] { };
            DefaultView = CmsViewNames.OnPageEditView;
        }
    }
}