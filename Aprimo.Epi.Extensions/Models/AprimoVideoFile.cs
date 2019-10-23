using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Aprimo.Epi.Extensions.Models
{
    [ContentType(DisplayName = "Aprimo Video File", GUID = "790c594c-5d04-4303-8da2-e38be10b2c19", Description = "")]
    [MediaDescriptor(Extensions = new[] { "AVI", "DV", "MPEG", "MPG", "MP4", "MOV", "WEBM", "WMV", "M4V" })]
    public class AprimoVideoFile : AprimoAssetBase, IContentVideo, IAprimoImage
    {
        [ScaffoldColumn(false)]
        public virtual string AprimoThumbnail { get; set; }

        [ScaffoldColumn(false)]
        public virtual string AprimoPreview { get; set; }
    }
}