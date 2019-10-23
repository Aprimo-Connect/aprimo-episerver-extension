namespace Aprimo.Epi.Extensions.Events
{
    public interface ICancelable
    {
        bool Cancelled { get; }

        void Cancel();
    }
}