using Aprimo.Epi.Extensions.Services;
using EPiServer;
using EPiServer.Framework.Blobs;
using EPiServer.PlugIn;
using EPiServer.Scheduler;
using System;
using System.Linq;

namespace Aprimo.Epi.Extensions.Implementation
{
    [ScheduledPlugIn(DisplayName = "Clean Remote Files Job", Description = "Cleans the remote property that holds the local file for view")]
    public class CleanRemoteFilesJob : ScheduledJobBase
    {
        private bool _stopSignaled;

        private readonly IAprimoAssetBlobMapService aprimoAssetBlobMappingService;

        private readonly IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository;

        private readonly IBlobFactory blobFactory;

        private readonly IContentCacheRemover cacheRemover;

        public CleanRemoteFilesJob(IAprimoAssetBlobMapService aprimoAssetBlobMappingService, IContentCacheRemover cacheRemover, IBlobFactory blobFactory, IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository, AprimoRESTService aprimoDamRepository)
        {
            IsStoppable = true;

            this.aprimoConnectorSettingsRepository = aprimoConnectorSettingsRepository;
            this.aprimoAssetBlobMappingService = aprimoAssetBlobMappingService;
            this.blobFactory = blobFactory;
            this.cacheRemover = cacheRemover;
        }

        /// <summary>
        /// Called when a user clicks on Stop for a manually started job, or when ASP.NET shuts down.
        /// </summary>
        public override void Stop()
        {
            _stopSignaled = true;
        }

        /// <summary>
        /// Called when a scheduled job executes
        /// </summary>
        /// <returns>A status message to be stored in the database log and visible from admin mode</returns>
        public override string Execute()
        {
            //Call OnStatusChanged to periodically notify progress of job for manually started jobs
            OnStatusChanged(String.Format("Starting execution of {0}", this.GetType()));
            var settings = this.aprimoConnectorSettingsRepository.Load();
            var mappings = this.aprimoAssetBlobMappingService.GetAll();

            int assetsUpdated = 0;
            int totalAssets = mappings.Count();
            if (mappings.Any())
            {
                foreach (var asset in mappings)
                {
                    bool updated = false;
                    if (asset.MasterUri != null)
                    {
                        var masterBlob = this.blobFactory.GetBlob(asset.MasterUri);
                        if (masterBlob != null)
                        {
                            this.blobFactory.Delete(masterBlob.ID);
                            asset.MasterUri = null;
                            updated = true;
                        }
                    }
                    if (asset.ThumbnailUri != null)
                    {
                        var thumbBlob = this.blobFactory.GetBlob(asset.ThumbnailUri);
                        if (thumbBlob != null)
                        {
                            this.blobFactory.Delete(thumbBlob.ID);
                            asset.ThumbnailUri = null;
                            updated = true;
                        }
                    }
                    if (updated)
                    {
                        this.aprimoAssetBlobMappingService.Save(asset);
                        this.cacheRemover.Remove(asset.ContentLink);
                        assetsUpdated++;
                    }
                    this.OnStatusChanged($"Assets {assetsUpdated} of {totalAssets}");
                }
            }

            return $"Assets {assetsUpdated} of {totalAssets}";
        }
    }
}