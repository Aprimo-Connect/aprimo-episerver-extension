using Aprimo.Epi.Extensions.API;
using Aprimo.Epi.Extensions.API.Assets;
using Aprimo.Epi.Extensions.API.Classifications;
using Aprimo.Epi.Extensions.Implementation;
using Aprimo.Epi.Extensions.Models;
using Aprimo.Epi.Extensions.Services;
using EPiServer.Construction;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Blobs;
using EPiServer.Logging.Compatibility;
using EPiServer.Security;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Epi.Extensions.Providers
{
    public class AprimoContentProvider : ContentProvider
    {
        #region Fields

        private readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IdentityMappingService identityMappingService;

        private readonly IAprimoRestService aprimoRestService;

        private readonly IContentTypeRepository contentTypeRepository;

        private readonly IContentFactory contentFactory;

        private readonly IUrlSegmentGenerator urlSegmentGenerator;

        private readonly IContentCacheKeyCreator contentCacheKeyCreator;

        private readonly ContentMediaResolver contentMediaResolver;

        private readonly IBlobFactory blobFactory;

        private readonly IAprimoAssetBlobMapService aprimoAssetBlobMappingService;

        private readonly IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository;

        private readonly AprimoConnectorSettings Settings;

        public ContentReference SearchResultNode { get; set; }

        #endregion Fields

        public AprimoContentProvider(IdentityMappingService identityMappingService, IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository, IAprimoAssetBlobMapService aprimoAssetBlobMappingService, ContentMediaResolver contentMediaResolver, IContentCacheKeyCreator contentCacheKeyCreator, IAprimoRestService aprimoDamRepository, IContentTypeRepository contentTypeRepository, IContentFactory contentFactory, IUrlSegmentGenerator urlSegmentGenerator, IBlobFactory blobFactory)
        {
            this.identityMappingService = identityMappingService;
            this.aprimoRestService = aprimoDamRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.contentFactory = contentFactory;
            this.urlSegmentGenerator = urlSegmentGenerator;
            this.contentCacheKeyCreator = contentCacheKeyCreator;
            this.contentMediaResolver = contentMediaResolver;
            this.blobFactory = blobFactory;
            this.aprimoAssetBlobMappingService = aprimoAssetBlobMappingService;
            this.aprimoConnectorSettingsRepository = aprimoConnectorSettingsRepository;
            this.Settings = this.aprimoConnectorSettingsRepository.Load();
        }

        /// <summary>
        /// Loads content from the external data store into IContent.
        /// </summary>
        /// <param name="contentLink">Content Reference used for determining the external content id to fetch content from.</param>
        /// <param name="languageSelector">If multilingual, the selector will determine the language to use.</param>
        /// <returns></returns>
        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            var mappedIdentity = this.identityMappingService.Get(contentLink);
            if (mappedIdentity != null)
            {
                var resourceId = (Models.AprimoResourceId)mappedIdentity.ExternalIdentifier;
                if (resourceId.ResourceType == this.contentTypeRepository.Load(typeof(ContentFolder)).Name)
                {
                    var classification = this.aprimoRestService.GetClassification(resourceId.CurrentItemId, AprimoEpiConstants.SelectClassificationFields);
                    return CreateContentFolder(mappedIdentity, resourceId, classification);
                }

                try
                {
                    var asset = this.aprimoRestService.GetAsset(resourceId.CurrentItemId, AprimoEpiConstants.SelectAssetFields);

                    var content = this.CreateContentWithIdentity(mappedIdentity, resourceId.ResourceType, resourceId, asset.Embedded.MasterFileVersion.FileName);
                    if (content is AprimoAssetBase aprimoAsset)
                        content = this.CreateAprimoFile(aprimoAsset, asset);
                    return content;
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        protected override IList<GetChildrenReferenceResult> LoadChildrenReferencesAndTypes(ContentReference contentLink, string languageID, out bool languageSpecific)
        {
            languageSpecific = false;

            var childrenList = new List<GetChildrenReferenceResult>();

            if (EntryPoint.CompareToIgnoreWorkID(contentLink))
            {
                string rootId = "null";
                if (!string.IsNullOrWhiteSpace(Settings.AprimoDAMARootClassificationId))
                    rootId = Settings.AprimoDAMARootClassificationId;
                childrenList.AddRange(this.GetClassifications(contentLink, rootId));

                return childrenList;
            }

            var mappedIdentity = this.identityMappingService.Get(contentLink);
            var contentResourceId = (AprimoResourceId)mappedIdentity.ExternalIdentifier;

            if (contentResourceId.ResourceType == this.contentTypeRepository.Load(typeof(ContentFolder)).Name)
            {
                var classifications = this.GetClassifications(mappedIdentity.ContentLink, contentResourceId.CurrentItemId);
                if (classifications.Any())
                    childrenList.AddRange(classifications);

                var assets = this.GetClassificationAssets(mappedIdentity.ContentLink, contentResourceId.CurrentItemId);
                if (assets.Any())
                    childrenList.AddRange(assets);
            }
            return childrenList;
        }

        #region Resolving Content

        /// <summary>
        /// Used to resolve Guid, IDs and urls.
        /// </summary>
        /// <param name="contentLink"></param>
        /// <returns></returns>
        protected override ContentResolveResult ResolveContent(ContentReference contentLink)
        {
            // Check to see fi this is our content
            if (contentLink.ProviderName != this.ProviderKey)
                return null;

            ContentResolveResult contentResolvedResult = new ContentResolveResult
            {
                ContentLink = contentLink
            };
            var content = LoadContent(contentLink, null);
            contentResolvedResult.UniqueID = content.ContentGuid;
            contentResolvedResult.ContentUri = ConstructContentUri(content.ContentTypeID, contentResolvedResult.ContentLink, contentResolvedResult.UniqueID);
            return contentResolvedResult;
        }

        protected override ContentResolveResult ResolveContent(Guid contentGuid)
        {
            var contentItem = this.identityMappingService.Get(contentGuid);
            if (contentItem == null)
                return null;

            ContentResolveResult contentResolvedType = new ContentResolveResult
            {
                ContentLink = contentItem.ContentLink
            };
            var content = LoadContent(contentResolvedType.ContentLink, null);
            contentResolvedType.UniqueID = contentGuid;
            contentResolvedType.ContentUri = ConstructContentUri(content.ContentTypeID, contentResolvedType.ContentLink, contentResolvedType.UniqueID);
            return contentResolvedType;
        }

        #endregion Resolving Content

        #region Creating Content Type Items

        private IContent CreateContentFolder(MappedIdentity mappedIdentity, AprimoResourceId resourceId, Classification classification) =>
            CreateContentWithIdentity(mappedIdentity, this.contentTypeRepository.Load(typeof(ContentFolder)), resourceId, classification.Name);

        private IContent CreateAprimoFile(AprimoAssetBase mediaFile, Asset asset)
        {
            mediaFile.AprimoId = asset.Id;
            mediaFile.Created = asset.CreatedOn.Date;
            mediaFile.StartPublish = asset.CreatedOn.Date;
            if (asset.Embedded != null)
            {
                var assetBlobMapping = this.aprimoAssetBlobMappingService.Find(asset.Id);
                if (mediaFile is IAprimoImage aprimoImageFile)
                {
                    // Sets External Preivew Url from Aprimo
                    if (asset.Embedded.Preview != null && asset.Embedded.Preview.Uri != null)
                        aprimoImageFile.AprimoPreview = asset.Embedded.Preview.Uri.AbsoluteUri;

                    // Sets External Thumbnail Url from Aprimo
                    if (asset.Embedded.Thumbnail != null && asset.Embedded.Thumbnail.Uri != null)
                        aprimoImageFile.AprimoThumbnail = asset.Embedded.Thumbnail.Uri.AbsoluteUri;

                    // Tries to get the local thumbnail copy, if it doesn't exist, it creates a new thumbnail from aprimo and saves the copy
                    // locally and uses it.
                    if (assetBlobMapping?.ThumbnailUri != null)
                    {
                        mediaFile.Thumbnail = this.blobFactory.GetBlob(assetBlobMapping.ThumbnailUri);
                    }
                    else
                    {
                        var thumbBlob = AprimoBlobHelper.DownloadThumbnail(mediaFile, asset);
                        assetBlobMapping = new AprimoAssetBlob()
                        {
                            ThumbnailUri = thumbBlob.ID,
                            ContentLink = mediaFile.ContentLink,
                            Id = Identity.NewIdentity(new Guid(mediaFile.AprimoId))
                        };
                        this.aprimoAssetBlobMappingService.Save(assetBlobMapping);
                        mediaFile.Thumbnail = this.blobFactory.GetBlob(thumbBlob.ID);
                    }
                }

                if (assetBlobMapping?.MasterUri != null)
                    mediaFile.BinaryData = this.blobFactory.GetBlob(assetBlobMapping.MasterUri);

                // Field Mapping
                if (asset.Embedded.Fields.Items.Any())
                {
                    // Set Expiration DateEPiServerExpirationDate
                    string expirationDate = asset.GetFieldValue(AprimoEpiConstants.AprimoEPiServerExpirationDate);
                    if (!string.IsNullOrWhiteSpace(expirationDate))
                    {
                        DateTime? expiration = DateTime.Parse(expirationDate);
                        if (expiration != null)
                            ((IVersionable)mediaFile).StopPublish = expiration;
                    }
                }

                this.AddContentToCache(mediaFile);
            }

            var contentType = this.contentTypeRepository.Load(mediaFile.ContentTypeID);
            if (contentType != null)
            {
                var dictionary = ReflectionHelpers.GetPropertyNameAndAttributeValue(contentType.ModelType);
                foreach (var kv in dictionary)
                {
                    mediaFile.SetValue(kv.Key, asset.GetFieldValue(kv.Value));
                }
            }

            return mediaFile;
        }

        public IContent CreateSearchResult(MappedIdentity mappedIdentity, Asset asset)
        {
            var resourceId = (AprimoResourceId)mappedIdentity.ExternalIdentifier;
            var content = this.CreateContentWithIdentity(mappedIdentity, resourceId.ResourceType, resourceId, asset.Embedded.MasterFileVersion.FileName);
            if (content is AprimoAssetBase aprimoAsset)
                content = this.CreateAprimoFile(aprimoAsset, asset);
            return content;
        }

        private IContent CreateContentWithIdentity(MappedIdentity mappedIdentity, string contentTypeName, AprimoResourceId resourceId, string name) =>
            this.CreateContentWithIdentity(mappedIdentity, this.contentTypeRepository.Load(contentTypeName), resourceId, name);

        private IContent CreateContentWithIdentity(MappedIdentity mappedIdentity, ContentType contentType, AprimoResourceId resourceId, string name)
        {
            var content = this.contentFactory.CreateContent(contentType);
            content.ContentTypeID = contentType.ID;
            content.ParentLink = resourceId.ParentResourceId;
            content.ContentGuid = mappedIdentity.ContentGuid;
            content.ContentLink = mappedIdentity.ContentLink;
            content.Name = name;

            (content as IRoutable).RouteSegment = this.urlSegmentGenerator.Create(content.Name);

            var securable = content as IContentSecurable;
            securable.GetContentSecurityDescriptor()
                .AddEntry(new AccessControlEntry(EveryoneRole.RoleName, AccessLevel.Read));

            if (content is IVersionable versionable)
            {
                versionable.IsPendingPublish = false;
                versionable.Status = VersionStatus.Published;
                versionable.StartPublish = DateTime.MinValue;
            }
            if (content is IChangeTrackable changeTrackable)
            {
                changeTrackable.Changed = DateTime.Now;
            }
            return content;
        }

        #endregion Creating Content Type Items

        #region Caching

        protected override void SetCacheSettings(ContentReference contentReference, IEnumerable<GetChildrenReferenceResult> children, CacheSettings cacheSettings)
        {
            cacheSettings.SlidingExpiration = TimeSpan.FromMinutes(10);
            base.SetCacheSettings(contentReference, children, cacheSettings);
        }

        #endregion Caching

        #region helpers

        private List<GetChildrenReferenceResult> GetClassifications(ContentReference parentContentLink, string classificationId)
        {
            var childrenList = new List<GetChildrenReferenceResult>();
            var classificationList = this.aprimoRestService.GetClassificationChildren(classificationId, AprimoEpiConstants.SelectClassificationFields);
            if (classificationList != null && classificationList.Items.Count > 0)
            {
                var contentFolder = this.contentTypeRepository.Load(typeof(ContentFolder));
                foreach (var classification in classificationList.Items)
                {
                    var resourceId = new AprimoResourceId(contentFolder.Name, parentContentLink, classification.Id);
                    var classUri = MappedIdentity.ConstructExternalIdentifier(ProviderKey, resourceId.ToString());
                    var identity = this.identityMappingService.Get(classUri, true);
                    childrenList.Add(new GetChildrenReferenceResult()
                    {
                        ContentLink = identity.ContentLink,
                        IsLeafNode = false,
                        ModelType = typeof(ContentFolder)
                    });
                    this.AddContentToCache(CreateContentFolder(identity, resourceId, classification));
                }
            }

            return childrenList;
        }

        private List<GetChildrenReferenceResult> GetClassificationAssets(ContentReference contentLink, string classificationId, int pageIndex = 1)
        {
            var leafList = new List<GetChildrenReferenceResult>();
            var assetList = this.aprimoRestService.GetClassificationAssets(classificationId, AprimoEpiConstants.SelectAssetFields, pageIndex);
            if (assetList != null && assetList.Items.Count > 0)
            {
                foreach (var asset in assetList.Items)
                {
                    var assetType = this.DetermineType(asset);
                    var resourceId = new AprimoResourceId(assetType.Name, contentLink, asset.Id);
                    if (asset.Embedded != null && asset.Embedded.MasterFileVersion != null)
                    {
                        var assetUri = MappedIdentity.ConstructExternalIdentifier(ProviderKey, resourceId.ToString());
                        var identity = this.identityMappingService.Get(assetUri, true);

                        leafList.Add(new GetChildrenReferenceResult()
                        {
                            ContentLink = identity.ContentLink,
                            IsLeafNode = true,
                            ModelType = assetType.ModelType
                        });

                        var content = this.CreateContentWithIdentity(identity, resourceId.ResourceType, resourceId, asset.Embedded.MasterFileVersion.FileName);
                        if (content is AprimoAssetBase aprimoAsset)
                            content = CreateAprimoFile(aprimoAsset, asset);
                    }
                }

                if ((assetList.Page * assetList.PageSize) < assetList.TotalCount)
                {
                    leafList.AddRange(GetClassificationAssets(contentLink, classificationId, pageIndex + 1));
                }
            }
            return leafList;
        }

        private ContentType DetermineType(Asset asset)
        {
            if (asset == null)
                return null;

            ContentType contentType = this.contentTypeRepository.Load(typeof(AprimoGenericFile));

            var fileExtension = asset?.Embedded?.MasterFileVersion?.FileExtension;
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                var types = this.contentMediaResolver.ListAllMatching(fileExtension);
                var selectedType = types.FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IAprimoAsset)));
                if (selectedType != null)
                    contentType = this.contentTypeRepository.Load(selectedType);
            }

            return contentType;
        }

        #endregion helpers
    }
}