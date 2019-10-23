using Aprimo.Epi.Extensions.API;
using Aprimo.Epi.Extensions.API.Assets;
using Aprimo.Epi.Extensions.API.Orders;
using Aprimo.Epi.Extensions.Implementation;
using Aprimo.Epi.Extensions.Models;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Logging.Compatibility;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Epi.Extensions.Services
{
    public static class AprimoBlobHelper
    {
        public static Injected<IBlobFactory> BlobFactory { get; set; }

        public static Injected<IAprimoRestService> AprimoRestService { get; set; }

        public static Injected<IAprimoAssetBlobMapService> AprimoAssetBlobMappingService { get; set; }

        public static Injected<IContentProviderManager> ContentProviderManager { get; set; }

        public static Injected<IContentCacheRemover> ContentCacheRemover { get; set; }

        public static Blob DownloadThumbnail(AprimoAssetBase aprimoAssetBase, bool overwriteCurrentThumbnail = false) =>
            DownloadThumbnail(aprimoAssetBase, AprimoRestService.Service.GetAsset(aprimoAssetBase.AprimoId, AprimoEpiConstants.SelectAssetFields), overwriteCurrentThumbnail);

        public static Blob DownloadThumbnail(AprimoAssetBase aprimoAssetBase, Asset asset, bool overwriteCurrentThumbnail = false)
        {
            if (aprimoAssetBase is IAprimoImage aprimoImage)
            {
                var assetFields = asset.Embedded.Fields;
                if (assetFields != null && assetFields.Items.Any())
                {
                    string extension = "jpg";
                    if (!string.IsNullOrWhiteSpace(asset.Embedded.Thumbnail?.Extension))
                        extension = asset.Embedded.Thumbnail.Extension;

                    var buffer = DownloadData(aprimoImage.AprimoThumbnail);
                    if (buffer != null)
                    {
                        var blob = BlobFactory.Service.CreateBlob(aprimoAssetBase.BinaryDataContainer, $".{extension}");
                        blob.WriteAllBytes(buffer);
                        var aprimoBlob = AprimoAssetBlobMappingService.Service.Find(aprimoAssetBase.AprimoId);
                        if (aprimoBlob == null)
                        {
                            aprimoBlob = new AprimoAssetBlob()
                            {
                                ContentLink = aprimoAssetBase.ContentLink,
                                Id = new Guid(aprimoAssetBase.AprimoId)
                            };
                        }
                        else
                        {
                            if (overwriteCurrentThumbnail)
                                if (aprimoBlob.ThumbnailUri != null)
                                    BlobFactory.Service.Delete(aprimoBlob.ThumbnailUri);
                        }

                        if (blob != null)
                        {
                            aprimoBlob.ThumbnailUri = blob.ID;
                            AprimoAssetBlobMappingService.Service.Save(aprimoBlob);
                            return blob;
                        }
                    }
                }
            }
            return null;
        }

        public static Blob DownloadMasterFile(AprimoAssetBase mediaFile, bool overwriteMasterFile = false)
        {
            if (mediaFile is IAprimoAsset aprimoAsset)
            {
                var file = CreateAndReturnOrder(mediaFile);
                if (!string.IsNullOrWhiteSpace(file))
                {
                    var asset = AprimoRestService.Service.GetAsset(mediaFile.AprimoId, AprimoEpiConstants.SelectAssetFields);
                    var assetFields = asset.Embedded.Fields;
                    if (assetFields != null && assetFields.Items.Any())
                    {
                        var extension = asset.Embedded.MasterFileVersion.FileExtension;
                        var buffer = DownloadData(file);
                        if (buffer != null)
                        {
                            var blob = BlobFactory.Service.CreateBlob(mediaFile.BinaryDataContainer, $".{extension}");
                            blob.WriteAllBytes(buffer);

                            // Get and Save blob information back to DDS
                            var aprimoBlob = AprimoAssetBlobMappingService.Service.Find(mediaFile.AprimoId);

                            if (aprimoBlob == null)
                            {
                                aprimoBlob = new AprimoAssetBlob()
                                {
                                    ContentLink = mediaFile.ContentLink,
                                    Id = new Guid(mediaFile.AprimoId)
                                };
                            }
                            else
                            {
                                if (overwriteMasterFile)
                                    if (aprimoBlob.MasterUri != null)
                                        BlobFactory.Service.Delete(aprimoBlob.MasterUri);
                            }
                            if (blob != null)
                            {
                                aprimoBlob.MasterUri = blob.ID;
                                AprimoAssetBlobMappingService.Service.Save(aprimoBlob);
                                ContentCacheRemover.Service.Remove(mediaFile.ContentLink);
                                var provider = ContentProviderManager.Service.GetProvider(AprimoEpiConstants.ProviderKey);
                                provider.Load(mediaFile.ContentLink, LanguageSelector.AutoDetect());
                                return blob;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static byte[] DownloadData(string fileUrl)
        {
            byte[] buffer = null;
            try
            {
                using (var client = new System.Net.WebClient())
                    buffer = client.DownloadData(fileUrl);
            }
            catch (Exception ex)
            {
            }

            return buffer;
        }

        private static string CreateAndReturnOrder(IAprimoAsset contentItem)
        {
            var createdOrder = new CreateOrder()
            {
                CreatorEmail = "episerver@aprimo.com",
                Type = "download",
                Targets = new List<CreateOrderTarget>() {
                    new CreateOrderTarget(){
                        AssetType = "LatestVersionOfMasterFile",
                        TargetTypes = new List<string>(){
                            "Document"
                        },
                        RecordId =contentItem.AprimoId
                    }
                }
            };
            var order = AprimoRestService.Service.CreateOrder(createdOrder);
            if (order != null)
            {
                int count = 0;
                while (count <= 20)
                {
                    var orderStatus = AprimoRestService.Service.GetOrderStatus(order.Id);
                    if (orderStatus.Status == "Success")
                    {
                        var files = orderStatus.DeliveredFiles;
                        return files.FirstOrDefault();
                    }
                    else
                    {
                        count++;
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            return string.Empty;
        }
    }
}