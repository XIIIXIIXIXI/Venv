using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Models.Interfaces
{
    public interface IDispatcherQueue
    {
        void TryEnqueue(DispatcherQueueHandler handler);
    }
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
