namespace Aprimo.Epi.Extensions
{
    /// <summary>
    /// Constants used for the Aprimo DAM System.
    /// </summary>
    public class AprimoEpiConstants
    {
        public const string ProviderKey = "aprimo";

        public const string ProviderName = "Aprimo";

        public const string SelectAssetFields = "preview, thumbnail, masterfilelatestversion, fields";

        public const string SelectClassificationFields = "masterfilelatestversion, fields, classifications";

        public const string AprimoAccessToken = "Aprimo-Access-Token";

        public const string AprimoEPiThumbnailFieldName = "EPiServerBlobThumbnailUri";

        public const string AprimoEPiMasterFileUriFieldName = "EPiServerBlobUri";

        public const string AprimoEPiPermalinkFieldName = "EPiServerPermalink";

        public const string FieldTokens = "EPiServerReferencedUris";

        public const string AprimoEPiServerExpirationDate = "EPiServerExpirationDate";
    }
}