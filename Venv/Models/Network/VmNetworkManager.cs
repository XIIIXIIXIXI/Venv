using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Venv.Models.DockerHandler;
using Venv.Models.DockerHandler.Interfaces;

namespace Venv.Models.Network
{
    public class VmNetworkManager
    {
        private readonly ISshClient _sshClient;
        private readonly string _desiredIP = "172.16.2.0";
        private readonly string _desiredNetmask = "255.255.0.0";
        private readonly string _interfaceName = "ens34";

        public VmNetworkManager(IPAddress vmIP)
        {
            _sshClient = new SshClient(vmIP, "vdpu");
        }

        public bool CheckAndConfigureNetwork()
        {
            if (!_sshClient.IsConnected)
            {
                _sshClient.Connect();
            }
            bool isConfiguredCorrectly = CheckNetworkConfiguration();
            if (!isConfiguredCorrectly)
            {
                ConfigureNetwork();
                return true; //changes was made
            }
            return false; //no changes made
        }

        private bool CheckNetworkConfiguration()
        {
            string checkCommand = "echo \"COMMAND_START\"; " +
        $"ip addr show {_interfaceName} | grep 'inet ' | awk '{{print $2}}'; echo \"COMMAND_END\"";
            _sshClient.ExecuteCommand(checkCommand);
            var reader = _sshClient.GetStandardOutput();
            string? line;       
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Equals("COMMAND_START"))
                {
                    break;
                }
            }
            string output = reader.ReadLine();
            if (output != null && output != "COMMAND_END")
            {
                var ipWithSubnet = output.Split('/');
                var ip = ipWithSubnet[0];
                if (ip.Contains(_desiredIP))
                {
                    return true;
                }
            }
            return false;
        }
        private void ConfigureNetwork()
        {
            string configureCommand = $"sudo ip addr flush dev {_interfaceName} && sudo ip addr add {_desiredIP}/16 dev {_interfaceName}";
            _sshClient.ExecuteCommand(configureCommand);
        }
    }
}
