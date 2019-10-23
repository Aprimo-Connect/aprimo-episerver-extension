using Aprimo.Epi.Extensions.Models;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using System;
using System.Linq;

namespace Aprimo.Epi.Extensions.Implementation
{
    /// <summary>
    /// Inserts our media data resolver instead of episerver's but we only care about Aprimo assets so it will only
    /// fire when it is an aprimo assets and fall through on the default.
    /// </summary>
    public class AprimoMediaDataResolver : ContentMediaResolver
    {
        public override void Initialize(ContentTypeModelRepository modelRepository)
        {
            base.Initialize(modelRepository);
        }

        public override Type GetFirstMatching(string extension)
        {
            var matches = this.ListAllMatching(extension);
            var siteDefault = matches.FirstOrDefault(x => !x.GetInterfaces().Contains(typeof(IAprimoAsset)));
            return siteDefault;
        }
    }
}