namespace Aprimo.Epi.Extensions.Models
{
    public interface IAprimoImage : IAprimoAsset
    {
        string AprimoThumbnail { get; set; }

        string AprimoPreview { get; set; }
    }
}