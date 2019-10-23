using Aprimo.Epi.Extensions.Events;
using Aprimo.Epi.Extensions.Models;
using Aprimo.Epi.Extensions.Services;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using System;

namespace Aprimo.Epi.Extensions.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(AprimoDAMInitialization))]
    public class AprimoDAMEventsInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            AprimoEventManager<IAprimoAsset>.Instance.OnPreviewingContent += Instance_OnPreviewingContent;
        }

        private void Instance_OnPreviewingContent(object sender, AprimoAssetEventArgs<IAprimoAsset> e)
        {
            if (e.Content is AprimoAssetBase aprimoAssetBase)
            {
                if (aprimoAssetBase.BinaryData?.ID == null)
                {
                    var mapping = ServiceLocator.Current.GetInstance<IAprimoAssetBlobMapService>();
                    var mappedItem = mapping.Find(x => x.Id.ExternalId.Equals(new Guid(aprimoAssetBase.AprimoId)));
                    if (mappedItem == null || (mappedItem != null && mappedItem.MasterUri == null))
                    {
                        AprimoBlobHelper.DownloadMasterFile(aprimoAssetBase, true);
                    }
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
            AprimoEventManager<IAprimoAsset>.Instance.OnPreviewingContent -= Instance_OnPreviewingContent;
        }
    }
}