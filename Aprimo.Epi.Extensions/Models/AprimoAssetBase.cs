using EPiServer.Core;
using System.ComponentModel.DataAnnotations;

namespace Aprimo.Epi.Extensions.Models
{
    public abstract class AprimoAssetBase : MediaData, IAprimoAsset
    {
        [ScaffoldColumn(false)]
        [Display(Name = "Aprimo Id", Order = 1)]
        public virtual string AprimoId { get; set; }
    }
}