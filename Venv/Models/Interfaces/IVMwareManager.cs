using System.Net;
using System.Threading.Tasks;

namespace Venv.Models.Interfaces
{
    public interface IVMwareManager
    {
        // Starts the VMware instance
        public void StartVMwareInstance();

        // Stops the VMware instance
        public void StopVMwareInstance();

        // Executes a command on the VMware instance
        Task ExecuteVMwareCommandAsync(string command);

        bool IsVMwareInstanceRunning { get; }

        public IPAddress IP { get; set; }

    }
}
