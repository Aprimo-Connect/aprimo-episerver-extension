using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using System;

namespace Aprimo.Epi.Extensions.Implementation
{
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true, StoreName = "Aprimo-Asset-Blob")]
    public class AprimoAssetBlob : IDynamicData
    {
        [EPiServerDataIndex]
        public Identity Id { get; set; }

        [EPiServerDataIndex]
        public ContentReference ContentLink { get; set; }

        public Uri ThumbnailUri { get; set; }

        public Uri MasterUri { get; set; }
    }
}