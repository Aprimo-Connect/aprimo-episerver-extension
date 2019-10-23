using Aprimo.Epi.Extensions.Models;
using System;

namespace Aprimo.Epi.Extensions.Events
{
    public class AprimoEventManager<T> where T : IAprimoAsset
    {
        private static AprimoEventManager<T> instance = null;

        public AprimoEventManager()
        { }

        public event EventHandler<AprimoAssetEventArgs<T>> OnPreviewingContent;

        public void TriggerOnPreviewingContent(object sender, AprimoAssetEventArgs<T> args)
        {
            AprimoEventHelper.Trigger(sender, args, OnPreviewingContent);
        }

        public event EventHandler<AprimoRESTAssetEventArgs> OnUpdatingRecord;

        public void TriggerOnUpdatingRecord(object sender, AprimoRESTAssetEventArgs args)
        {
            AprimoEventHelper.Trigger(sender, args, OnUpdatingRecord);
        }

        public static AprimoEventManager<T> Instance
        {
            get
            {
                return (instance ?? (instance = new AprimoEventManager<T>()));
            }

            internal set
            {
                instance = value;
            }
        }
    }
}