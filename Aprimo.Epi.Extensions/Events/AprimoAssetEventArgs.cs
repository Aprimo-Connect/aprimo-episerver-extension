using Aprimo.Epi.Extensions.Models;
using System;

namespace Aprimo.Epi.Extensions.Events
{
    public class AprimoAssetEventArgs<T> : EventArgs, ICancelable where T : IAprimoAsset
    {
        private T content;

        private bool cancel;

        // Methods
        public AprimoAssetEventArgs(T content) => this.content = content;

        public bool Cancelled => this.cancel;

        public void Cancel() => this.cancel = true;

        public T Content => content;
    }
}