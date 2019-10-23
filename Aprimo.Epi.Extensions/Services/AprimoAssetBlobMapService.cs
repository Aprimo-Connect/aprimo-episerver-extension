using Aprimo.Epi.Extensions.Implementation;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Epi.Extensions.Services
{
    [ServiceConfiguration(ServiceType = typeof(IAprimoAssetBlobMapService), Lifecycle = ServiceInstanceScope.Transient)]
    public class AprimoAssetBlobMapService : IAprimoAssetBlobMapService
    {
        protected DynamicDataStore GetDynamicDataStore()
        {
            return DynamicDataStoreFactory.Instance.GetStore(typeof(AprimoAssetBlob)) ??
                  DynamicDataStoreFactory.Instance.CreateStore(typeof(AprimoAssetBlob));
        }

        public AprimoAssetBlob Find(Guid id)
        {
            using (var store = GetDynamicDataStore())
                return store.Load<AprimoAssetBlob>(Identity.NewIdentity(id));
        }

        public AprimoAssetBlob Find(string assetId) =>
            this.Find(new Guid(assetId));

        public AprimoAssetBlob Find(Func<AprimoAssetBlob, bool> func)
        {
            using (var store = GetDynamicDataStore())
                return store.Items<AprimoAssetBlob>()
                          .FirstOrDefault(func);
        }

        public void Save(AprimoAssetBlob asset)
        {
            using (var store = GetDynamicDataStore())
                store.Save(asset);
        }

        public void Delete(AprimoAssetBlob asset)
        {
            using (var store = GetDynamicDataStore())
                store.Save(asset);
        }

        public void Delete(Guid assetId)
        {
            var asset = this.Find(assetId);
            if (asset != null)
                this.Delete(asset);
        }

        public void DeleteAll()
        {
            using (var store = GetDynamicDataStore())
                store.DeleteAll();
        }

        public IEnumerable<AprimoAssetBlob> GetAll()
        {
            using (var store = GetDynamicDataStore())
                return store.Items<AprimoAssetBlob>();
        }

        public IEnumerable<AprimoAssetBlob> GetItems(Func<AprimoAssetBlob, bool> func)
        {
            using (var store = GetDynamicDataStore())
                return store.Items<AprimoAssetBlob>().Where(func);
        }
    }
}