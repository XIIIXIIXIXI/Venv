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
        ContainerManager _containerManager;

        public Mediator(ShipDataService shipDataService, VMwareManager vMwareManager)
        {
            _vmwareManager = vMwareManager;
            _shipDataService = shipDataService;
            _ = ConnectToVM();
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
                    _containerManager = new ContainerManager(_sshClient, _shipDataService);
                    _sshClient.Connect();
                }
            });  
        }

        public async Task StartDockerContainersAsync()
        {
            var monitorTask = MonitorDockerEventsAsync(null);
            List<int> dpuNumbers = _shipDataService.GetSelectedDpus().Select(dpu => dpu.Number).ToList();
            string dpusArgument = string.Join(" ", dpuNumbers);
            _sshClient.ExecuteCommand($"./ManageDockers.sh {dpusArgument}");
            await monitorTask;
        }
        public async Task StopDockerContainersAsync()
        {
            var monitorTask = MonitorDockerEventsAsync(null);
            _sshClient.ExecuteCommand("docker compose down");
            await monitorTask;
        }

        public async Task MonitorDockerEventsAsync(DPU? singleDPU)
        {
            await _containerManager.ListenForEventsAsync(singleDPU);
        }
    }
}
