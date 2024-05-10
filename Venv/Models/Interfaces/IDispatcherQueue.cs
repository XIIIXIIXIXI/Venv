using Microsoft.UI.Dispatching;
using System.Diagnostics.CodeAnalysis;


namespace Venv.Models.Interfaces
{

    public interface IDispatcherQueue
    {
        void TryEnqueue(DispatcherQueueHandler handler);
    }
    [ExcludeFromCodeCoverage]
    public class DispatcherQueueWrapper : IDispatcherQueue
    {
        private readonly DispatcherQueue _dispatcherQueue;

        public DispatcherQueueWrapper()
        {
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        }

        public void TryEnqueue(DispatcherQueueHandler handler)
        {
            _dispatcherQueue.TryEnqueue(handler);
        }
    }
}
