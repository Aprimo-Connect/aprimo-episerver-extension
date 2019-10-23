using Aprimo.Epi.Extensions.API;
using Aprimo.Epi.Extensions.API.Assets;
using Aprimo.Epi.Extensions.API.Classifications;
using Aprimo.Epi.Extensions.API.Orders;
using Aprimo.Epi.Extensions.API.Search;
using EPiServer.Framework.Cache;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Aprimo.Epi.Extensions.Services
{
    [ServiceConfiguration(ServiceType = typeof(IAprimoRestService), Lifecycle = ServiceInstanceScope.Transient)]
    public class AprimoRESTService : IAprimoRestService
    {
        private readonly ISynchronizedObjectInstanceCache cacheManager;

        private readonly IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository;

        private readonly AprimoConnectorSettings Settings;

        public AprimoRESTService(ISynchronizedObjectInstanceCache cacheManager, IAprimoConnectorSettingsRepository aprimoConnectorSettingsRepository)
        {
            this.cacheManager = cacheManager;
            this.aprimoConnectorSettingsRepository = aprimoConnectorSettingsRepository;
            this.Settings = this.aprimoConnectorSettingsRepository.Load();
        }

        /// <summary>
        /// Gets a single classification
        /// </summary>
        /// <param name="id">The id of the classification you are attempting to retrieve.</param>
        /// <param name="selectFields">A list of string parameters seperated by comma's you want returned.</param>
        /// <returns>Classification</returns>
        public virtual Classification GetClassification(string id, string selectFields)

        {
            var request = new RestRequest("api/core/classification/{classification}", Method.GET)
                .AddParameter("classification", id, ParameterType.UrlSegment)
                .AddHeader("select-record", selectFields);
            var classification = this.GetFromRest<Classification>(request);
            return classification;
        }

        /// <summary>
        /// Returns a  list of classifications by parent id
        /// </summary>
        /// <param name="parentClassificationId">Parent id of the the classification</param>
        /// <param name="selectFields">A list of string parameters seperated by comma's you want returned.</param>
        /// <returns>ClassificationList</returns>
        public virtual ClassificationList GetClassificationChildren(string parentClassificationId, string selectFields)
        {
            if (string.IsNullOrWhiteSpace(parentClassificationId))
                return new ClassificationList();

            var request = new RestRequest("api/core/classifications", Method.GET)
                    .AddParameter("filter", $"parent.id={parentClassificationId}")
                    .AddHeader("select-record", selectFields);
            var classifications = this.GetFromRest<ClassificationList>(request);
            return classifications;
        }

        /// <summary>
        /// Gets a single asset
        /// </summary>
        /// <param name="id">The id of the asset you are attempting to retrieve</param>
        /// <param name="selectFields">The fields you expect returned</param>
        /// <returns>Asset</returns>
        public virtual Asset GetAsset(string id, string selectFields)
        {
            var request = new RestRequest("api/core/record/{recordId}", Method.GET)
                .AddParameter("recordId", id, ParameterType.UrlSegment)
                .AddHeader("select-record", selectFields);

            var asset = this.GetFromRest<Asset>(request);
            return asset;
        }

        /// <summary>
        /// Retrieves an asset.
        /// </summary>
        /// <param name="classificationId">Classification ID</param>
        /// <param name="selectFields">A list of string parameters seperated by comma's you want returned.</param>
        /// <returns>AsssetList</returns>
        public virtual AssetList GetClassificationAssets(string classificationId, string selectFields, int pageIndex = 1, int pageSize = 50)
        {
            var assetList = this.GetAssets(new SearchRequest()
            {
                LogRequest = true,
                PageIndex = pageIndex,
                PageSize = pageSize,
                SearchExpression = new SearchExpression()
                {
                    Expression = $"DirectClassification.Id={classificationId} FileCount > 0"
                }
            }, selectFields);

            return assetList;
        }

        /// <summary>
        /// Retrieves an asset.
        /// </summary>
        /// <param name="classificationId">Classification ID</param>
        /// <param name="selectFields">A list of string parameters seperated by comma's you want returned.</param>
        /// <returns>AsssetList</returns>
        public virtual List<Asset> GetClassificationAssetsDescendants(string classificationId, int pageSize = 100) =>
            this.GetAssets(1, pageSize);

        /// <summary>
        /// Gets a list of of assets based on the populated searchrequest object
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <param name="selectFields">The fields you expect returned</param>
        /// <returns>AssetList</returns>
        public virtual AssetList GetAssets(SearchRequest searchRequest, string selectFields)
        {
            var request = new RestRequest("api/core/search/records", Method.POST)
                .AddHeader("select-record", selectFields)
                .AddHeader("page", searchRequest.PageIndex.ToString())
                .AddHeader("pageSize", searchRequest.PageSize.ToString())
                .AddJsonBody(searchRequest);

            var assets = this.GetFromRest<AssetList>(request);
            return assets;
        }

        /// <summary>
        /// Creates an order
        /// </summary>
        /// <param name="createOrder"></param>
        /// <returns></returns>
        public virtual Order CreateOrder(CreateOrder createOrder)
        {
            var order = new Order();
            var request = new RestRequest("api/core/orders", Method.POST)
              .AddJsonBody(createOrder);

            order = this.GetFromRest<Order>(request);
            return order;
        }

        /// <summary>
        /// Retrieves the status to the order.
        /// </summary>
        /// <param name="id">The id of the asset you are attempting to retrieve</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderStatus(string id)
        {
            var request = new RestRequest("api/core/order/{orderid}", Method.GET)
                .AddParameter("orderid", id, ParameterType.UrlSegment);
            var order = this.GetFromRest<Order>(request);
            return order;
        }

        /// <summary>
        /// Adds or updates the record in aprimo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="record"></param>
        public virtual void AddOrUpdate(string id, AddUpdateRecordRequest record)
        {
            var request = new RestRequest("api/core/record/{recordId}", Method.PUT)
                .AddParameter("recordId", id, ParameterType.UrlSegment)
                .AddJsonBody(record);

            var response = this.GetFromRest<Order>(request);
        }

        /// <summary>
        /// Retrieves the binary data from aprimo file.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="imageType"></param>
        /// <returns></returns>
        public virtual byte[] RetrieveImage(string id, ImageType imageType)
        {
            var auth = this.GetOrCreateToken();
            var client = new RestClient($"https://{Settings.AprimoTenantId}.dam.aprimo.com");
            var request = new RestRequest("api/core/record/{recordId}/image/{imagetype}/download", Method.GET)
                .AddParameter("recordId", id, ParameterType.UrlSegment)
                .AddParameter("imagetype", imageType.ToString(), ParameterType.UrlSegment)
                .AddHeader("Content-Type", "application/json;charset=utf-8")
                .AddHeader("Authorization", $"Bearer {auth.AccessToken}")
                .AddHeader("API-VERSION", Settings.AprimoDAMApiVersion)
                .AddHeader("Accept", "application/hal+json");

            var response = client.DownloadData(request);
            return response;
        }

        public virtual void ClearFields(string id, IEnumerable<string> fieldIds)
        {
            var items = new Fields();
            fieldIds
                .ToList()
                .ForEach(x => items.Add(x, string.Empty));

            this.AddOrUpdate(id, new AddUpdateRecordRequest(items));
        }

        public virtual void ClearFieldsByName(string id, IEnumerable<string> fieldNames)
        {
            if (fieldNames.Any())
            {
                var asset = this.GetAsset(id, AprimoEpiConstants.SelectAssetFields);
                this.ClearFieldsByName(asset, fieldNames);
            }
        }

        public virtual void ClearFieldsByName(Asset asset, IEnumerable<string> fieldNames)
        {
            if (fieldNames.Any())
            {
                if (asset?.Embedded?.Fields != null)
                {
                    var ids = asset.Embedded.Fields.Items.Where(x => fieldNames.Contains(x.FieldName))
                        .Select(x => x.Id)
                        .ToList();
                    if (ids.Any())
                        this.ClearFields(asset.Id, ids);
                }
            }
        }

        /// <summary>
        /// A Generic Rest method for making calls to the Aprimo DAM api.
        /// </summary>
        /// <typeparam name="T">A class object that is returned</typeparam>
        /// <param name="request">RestSharp IRequest object</param>
        /// <returns>T which is a class</returns>
        private T GetFromRest<T>(IRestRequest request) where T : class
        {
            var auth = this.GetOrCreateToken();
            var client = new RestClient($"https://{Settings.AprimoTenantId}.dam.aprimo.com");

            request
                .AddHeader("Content-Type", "application/json;charset=utf-8")
                .AddHeader("Authorization", $"Bearer {auth.AccessToken}")
                .AddHeader("API-VERSION", Settings.AprimoDAMApiVersion)
                .AddHeader("Accept", "application/hal+json");

            var response = client.Execute(request);
            if ((response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created) && !string.IsNullOrWhiteSpace(response.Content))
            {
                var responseItem = JsonConvert.DeserializeObject<T>(response.Content);
                return responseItem;
            }
            return null;
        }

        /// <summary>
        /// Creates or gets an Arimo Access token for making api calls.
        /// </summary>
        /// <returns>APrimoAuthorization Object</returns>
        private AprimoAuthorization GetOrCreateToken()
        {
            var aprimoAuthorization = this.cacheManager.Get<AprimoAuthorization>(AprimoEpiConstants.AprimoAccessToken, ReadStrategy.Immediate);
            if (aprimoAuthorization != null)
                return aprimoAuthorization;

            var client = new RestClient($"https://{Settings.AprimoTenantId}.aprimo.com");
            var request = new RestRequest("api/oauth/create-native-token", Method.POST)
                .AddHeader("Content-Type", "application/json;charset=utf-8")
                .AddHeader("client-id", Settings.AprimoDAMClientId)
                .AddHeader("Authorization", $"Basic {GenerateAccessToken()}");

            var response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK && !string.IsNullOrWhiteSpace(response.Content))
            {
                var responseItem = JsonConvert.DeserializeObject<AprimoAuthorization>(response.Content);
                this.cacheManager.Insert(AprimoEpiConstants.AprimoAccessToken, responseItem, new CacheEvictionPolicy(TimeSpan.FromMinutes(9), CacheTimeoutType.Absolute));
                return responseItem;
            }
            return null;
        }

        /// <summary>
        /// Generates a token UserToken based on accountname and account token that is base64
        /// </summary>
        /// <returns>string</returns>
        private string GenerateAccessToken()
        {
            var usernameToken = $"{Settings.AprimoDAMUsername}:{Settings.AprimoDAMUserToken}".Trim();
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(usernameToken);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private List<Asset> GetAssets(int index = 1, int pageSize = 100)
        {
            var assets = new List<Asset>();

            var assetList = this.GetAssets(new API.Search.SearchRequest()
            {
                PageIndex = index,
                LogRequest = true,
                PageSize = pageSize,
                SearchExpression = new API.Search.SearchExpression()
                {
                    Expression = $"Classification.Id={Settings.AprimoDAMARootClassificationId}"
                }
            }, AprimoEpiConstants.SelectAssetFields);
            assets.AddRange(assetList.Items);

            if ((assetList.Page * assetList.PageSize) < assetList.TotalCount)
                assets.AddRange(GetAssets(index + 1, pageSize));

            return assets;
        }
    }
}