using System.Net;
using System.Threading;
using Venv.Models.DockerHandler;
using Venv.Models.Interfaces;

namespace Venv.Models.Services
{
    public class VmNetworkManager
    {
        private readonly ISshClient _sshClient;
        private readonly string _desiredIP = "172.16.2.0";
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
            string line;
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
            string flushCommand = $"sudo ip addr flush dev {_interfaceName}";
            _sshClient.ExecuteCommand(flushCommand);

            string configureCommand = $"sudo ip link set dev {_interfaceName} mtu 1500 && " +
                              $"sudo ip link set dev {_interfaceName} up && " +
                              $"sudo ip addr add 172.16.2.0/16 brd 172.16.255.255 scope global noprefixroute dev {_interfaceName}";
            _sshClient.ExecuteCommand(configureCommand);

            Thread.Sleep(1000);
        }
    }
}
