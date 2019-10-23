using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aprimo.Epi.Extensions.Providers
{
    [ServiceConfiguration(typeof(IContentRepositoryDescriptor))]
    public class AprimoRepositoryDescriptor : ContentRepositoryDescriptorBase
    {
        private readonly IContentProviderManager providerManager;

        public AprimoRepositoryDescriptor(IContentProviderManager providerManager)
        {
            this.providerManager = providerManager;
        }

        public override IEnumerable<ContentReference> Roots =>
            new ContentReference[] { this.providerManager.GetProvider(AprimoEpiConstants.ProviderKey).EntryPoint };

        public static string RepositoryKey =>
            AprimoEpiConstants.ProviderKey;

        public override string Key =>
            AprimoEpiConstants.ProviderKey;

        public override string Name =>
            AprimoEpiConstants.ProviderKey;

        public override IEnumerable<Type> ContainedTypes =>
            new[] { typeof(MediaData) };

        public override IEnumerable<Type> CreatableTypes =>
            Enumerable.Empty<Type>();

        public override IEnumerable<Type> MainNavigationTypes =>
            new[] { typeof(ContentFolder) };
    }
}