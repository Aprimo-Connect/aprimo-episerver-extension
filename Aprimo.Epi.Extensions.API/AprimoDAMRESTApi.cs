using Aprimo.Epi.Extensions.API.Assets;
using Aprimo.Epi.Extensions.API.Classifications;
using Aprimo.Epi.Extensions.API.Orders;
using Aprimo.Epi.Extensions.API.Search;
using System.Collections.Generic;

namespace Aprimo.Epi.Extensions.API
{
    public interface IAprimoRestService
    {
        Classification GetClassification(string id, string selectFields);

        ClassificationList GetClassificationChildren(string parentClassificationId, string selectFields);

        Asset GetAsset(string id, string selectFields);

        /// <summary>
        /// Returns assets based on the classification id.
        /// </summary>
        /// <param name="classificationId"></param>
        /// <param name="selectFields"></param>
        /// <param name="pageIndex">Default to 1</param>
        /// <param name="pageSize">Default to 50</param>
        /// <returns>AssetList</returns>
        AssetList GetClassificationAssets(string classificationId, string selectFields, int pageIndex = 1, int pageSize = 50);

        List<Asset> GetClassificationAssetsDescendants(string classificationId, int pageSize = 100);

        AssetList GetAssets(SearchRequest searchRequest, string selectFields);

        Order CreateOrder(CreateOrder createOrder);

        Order GetOrderStatus(string id);

        byte[] RetrieveImage(string id, ImageType imageType);

        void AddOrUpdate(string id, AddUpdateRecordRequest record);

        void ClearFields(string id, IEnumerable<string> fieldIds);

        void ClearFieldsByName(string id, IEnumerable<string> fieldNames);

        void ClearFieldsByName(Asset asset, IEnumerable<string> fieldNames);
    }
}