using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace Aprimo.Epi.Extensions.Models
{
    [ContentType(DisplayName = "Pdf File", GUID = "ef832d23-51f3-4912-8bcb-8617628a2851", Description = "")]
    [MediaDescriptor(ExtensionString = "pdf")]
    public class AprimoPdfFile : AprimoAssetBase
    {
    }
}