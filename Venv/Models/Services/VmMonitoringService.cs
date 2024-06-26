﻿using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Venv.Models.DockerHandler;
using Venv.Models.Interfaces;

namespace Venv.Models.Services
{
    public class VmMonitoringService
    {
        private readonly ISshClient _sshClient;

        public VmMonitoringService(IPAddress vmIP)
        {
            _sshClient = new SshClient(vmIP, "vdpu");
        }
        public async Task<VmPerformanceData> GetPerformanceDataAsync()
        {
            if (!_sshClient.IsConnected)
            {
                _sshClient.Connect();
            }
            // Sometimes trash is returned from the VM. Echo a COMMAND_START to filter useless information
            string cpuUsageCommand = "echo \"COMMAND_START\"; " +
                "top -bn1 | grep \"Cpu(s)\" | awk '{print $2 + $4 \"%\"}'; " +
                "free -m | awk '/Mem:/ {used=$3; total=$2; printf \"%d\\n%dMB\\nMemory Usage: %.2f%%\\n\", used, total, used/total * 100}'";

            _sshClient.ExecuteCommand(cpuUsageCommand);

            var reader = _sshClient.GetStandardOutput();
            string line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.Equals("COMMAND_START"))
                {
                    break;
                }
            }
            var cpuUsage = await reader.ReadLineAsync();
            var cpuUsagePercent = ExtractPercentage(cpuUsage);
            var usedMemory = await reader.ReadLineAsync();
            var totalMemory = await reader.ReadLineAsync();
            var memoryUsage = await reader.ReadLineAsync();
            var memoryUsagePercent = ExtractPercentage(memoryUsage);

            return new VmPerformanceData
            {
                CpuUsage = cpuUsagePercent,
                usedMemory = usedMemory,
                totalMemory = totalMemory,
                memoryPercent = memoryUsagePercent
            };
        }
        private double ExtractPercentage(string input)
        {
            var match = Regex.Match(input, @"\d+\.?\d*");
            if (double.TryParse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return 0;
        }

        public async Task StopMonitoringAsync()
        {
            if (_sshClient.IsConnected)
            {
               await _sshClient.DisconnectAsync();
            }
        }
        public void StartMonitoring()
        {
            _sshClient.Connect();
        }

    }
    public class VmPerformanceData
    {
        public double CpuUsage { get; set; }
        public string usedMemory { get; set; }
        public string totalMemory { get; set; }
        public double memoryPercent { get; set; }
    }
}
