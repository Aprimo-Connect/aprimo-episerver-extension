using EPiServer.Core;

namespace Aprimo.Epi.Extensions.Models
{
    public interface IAprimoAsset : IContent
    {
        string AprimoId { get; set; }
    }
}