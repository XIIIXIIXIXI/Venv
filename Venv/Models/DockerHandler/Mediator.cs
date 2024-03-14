using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Venv.Models.DockerHandler.Interfaces;
using Venv.Services;

namespace Venv.Models.DockerHandler
{
    public class Mediator
    {
        private readonly IVMwareManager _vmwareManager;
        private ISSHClient _sshClient;
        private readonly ShipDataService _shipDataService;

        public Mediator(ShipDataService shipDataService, VMwareManager vMwareManager)
        {
            _vmwareManager = vMwareManager;
            _shipDataService = shipDataService;
        }

        public async Task ConnectToVM()
        {
            const int intervalMs = 1000;
            await Task.Run(async () =>
            {
                while(!_vmwareManager.IsVMwareInstanceRunning)
                {
                    await Task.Delay(intervalMs);
                }
                if (_vmwareManager.IsVMwareInstanceRunning)
                {
                    _sshClient = new SSHClient(_vmwareManager.IP, "vdpu");
                    //_containerManager = new ContainersManager(_sshClientWrapper, _xmlDataService);
                    _sshClient.Connect();
                }
            });  
        }
    }
}
