using System;
using System.Net;
using System.Threading.Tasks;

namespace Venv.Models.Interfaces
{
    public interface IVMwareManager
    {
        public void StartVMwareInstance();

        public void StopVMwareInstance();

        Task ExecuteVMwareCommandAsync(string command);

        bool IsVMwareInstanceRunning { get; }

        public IPAddress IP { get; set; }

        public event EventHandler<bool> VMStatusChanged;
        public void StartHeartBeat();

    }
}
