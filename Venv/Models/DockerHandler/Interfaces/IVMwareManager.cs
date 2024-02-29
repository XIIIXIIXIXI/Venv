using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Models.DockerHandler.Interfaces
{
    public interface IVMwareManager
    {
        // Starts the VMware instance
        public IPAddress StartVMwareInstance();

        // Stops the VMware instance
        Task StopVMwareInstanceAsync();

        // Executes a command on the VMware instance
        Task ExecuteVMwareCommandAsync(string command);

        bool IsVMwareInstanceRunning { get; }
    }
}
