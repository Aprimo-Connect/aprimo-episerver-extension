using Aprimo.Epi.Extensions.Attributes;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Aprimo.Epi.Extensions.Models
{
    [ContentType(DisplayName = "Aprimo Image File", GUID = "3e47e30f-cbbc-48d4-8d53-8d7784035919", Description = "")]
    [MediaDescriptor(Extensions = new[] { "JPEG", "JPG", "GIF", "PNG", "SVG", "TIFF", "WEBM", "BMP", "M4V", "PPM", "PGM", "PBM", "PNM", "WEBP", "HDR" })]
    public class AprimoImageFile : AprimoAssetBase, IContentImage, IAprimoImage
    {
        [AprimoFieldName("Record Title")]
        [Display(Name = "Title", Order = 2)]
        public virtual string Title { get; set; }

        [AprimoFieldName("Description")]
        [Display(Name = "Description", Order = 3)]
        public virtual string Description { get; set; }

        [ScaffoldColumn(false)]
        [Display(Order = 998)]
        public virtual string AprimoThumbnail { get; set; }

        [ScaffoldColumn(false)]
        [Display(Order = 999)]
        public virtual string AprimoPreview { get; set; }
    }
}