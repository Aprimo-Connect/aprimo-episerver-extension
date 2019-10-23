using Aprimo.Epi.Extensions.Implementation;
using System;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions.Services
{
    public interface IAprimoAssetBlobMapService
    {
        /// <summary>
        /// Finds the id in the store with this assetId.
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        AprimoAssetBlob Find(Guid assetId);

        /// <summary>
        /// Converts string to guid then attempts to find the matching store item identity.
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        AprimoAssetBlob Find(string assetId);

        AprimoAssetBlob Find(Func<AprimoAssetBlob, bool> func);

        void Save(AprimoAssetBlob asset);

        void Delete(AprimoAssetBlob asset);

        void Delete(Guid assetId);

        void DeleteAll();

        IEnumerable<AprimoAssetBlob> GetItems(Func<AprimoAssetBlob, bool> func);

        IEnumerable<AprimoAssetBlob> GetAll();
    }
}