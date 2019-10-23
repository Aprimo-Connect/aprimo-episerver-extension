using Aprimo.Epi.Extensions.API.Assets;
using System;

namespace Aprimo.Epi.Extensions.Events
{
    public class AprimoRESTAssetEventArgs : EventArgs, ICancelable
    {
        private Asset content;

        private bool cancel;

        // Methods
        public AprimoRESTAssetEventArgs(Asset asset)
            => this.content = asset;

        public bool Cancelled => this.cancel;

        public void Cancel() => this.cancel = true;

        public Asset Content => content;
    }
}