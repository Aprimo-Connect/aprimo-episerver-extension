using EPiServer.Core;
using System;
using System.Linq;

namespace Aprimo.Epi.Extensions.Models
{
    public class AprimoResourceId
    {
        public AprimoResourceId()
        {
            this.ParentResourceId = ContentReference.EmptyReference;
        }

        public AprimoResourceId(string resourceType, ContentReference parentContentLink, string aprimoAssetId)
        {
            this.ResourceType = resourceType;
            this.ParentResourceId = parentContentLink;
            this.CurrentItemId = aprimoAssetId;
        }

        public string ResourceType { get; set; }

        public ContentReference ParentResourceId { get; set; }

        public string CurrentItemId { get; set; }

        public static implicit operator string(AprimoResourceId id) =>
            id.ToString();

        public static explicit operator AprimoResourceId(Uri id)
        {
            if (id == null)
                return null;

            var resource = new AprimoResourceId() { ResourceType = typeof(AprimoGenericFile).Name };
            resource.ResourceType = RemoveTrailingSlash(id.Segments[1]);
            resource.ParentResourceId = new ContentReference(RemoveTrailingSlash(id.Segments[2].Trim()));
            resource.CurrentItemId = id.Segments.LastOrDefault().Trim();

            return resource;
        }

        public override string ToString() =>
            $"{this.ResourceType}/{ParentResourceId.ToReferenceWithoutVersion()}/{CurrentItemId}";

        private static string RemoveTrailingSlash(string virtualPath) =>
            !string.IsNullOrEmpty(virtualPath) && virtualPath[virtualPath.Length - 1] == '/' ? virtualPath.Substring(0, virtualPath.Length - 1) : virtualPath;
    }
}