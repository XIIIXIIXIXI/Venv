using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Venv.Models.Interfaces;
using Venv.Resources;

namespace Venv.Models.Services
{
    public class VMwareManager : IVMwareManager
    {
        private bool _isVMwareInstanceRunning { get; set; }
        public event EventHandler<bool> VMStatusChanged;
        public IPAddress IP { get; set; }
        public bool IsVMwareInstanceRunning => _isVMwareInstanceRunning;
        public readonly int HeartbeatInterval = 5000; //5 seconds 
        private VmNetworkManager _vmNetwork;

        private CancellationTokenSource _heartbeatCancellationTokenSource;

        public VMwareManager()
        {
            _isVMwareInstanceRunning = false;
        }

        public Task ExecuteVMwareCommandAsync(string command)
        {
            throw new NotImplementedException();
        }

        public void StartVMwareInstance()
        {

            if (!_isVMwareInstanceRunning)
            {
                //VM is starting
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = VMPaths.vmrunPath,
                    Arguments = $"-T ws start \"{VMPaths.vmxPath}\" nogui",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process.Start(startInfo);
                WaitingForVMToBeTurnedOn();
                _vmNetwork = new VmNetworkManager(IP);
                _vmNetwork.CheckAndConfigureNetwork();
                _isVMwareInstanceRunning = true;
                VMStatusChanged?.Invoke(this, _isVMwareInstanceRunning);

            }
            else
            {
                throw new InvalidOperationException("VMware instance is already running.");
            }
        }

        private void WaitingForVMToBeTurnedOn()
        {
            const int maxRetries = 600; // Maximum number of retries
            int retryCount = 0;

            while (!_isVMwareInstanceRunning && retryCount < maxRetries)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = VMPaths.vmrunPath,
                    Arguments = $"GetGuestIPAddress \"{VMPaths.vmxPath}\" -wait",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (Process process = Process.Start(startInfo))
                {
                    string output = process.StandardOutput.ReadToEnd();

                    if (!string.IsNullOrWhiteSpace(output) && output.Contains("The virtual machine is not powered on"))
                    {
                        // VM not powered on yet, wait and retry
                        Thread.Sleep(1000);
                        retryCount++;
                    }
                    else if (IPAddress.TryParse(output.Trim(), out var ip))
                    {
                        IP = ip;
                        return;
                    }
                }
            }
        }

        public void StartHeartBeat()
        {
            _heartbeatCancellationTokenSource = new CancellationTokenSource();
            var token = _heartbeatCancellationTokenSource.Token;


            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    var previousStatus = _isVMwareInstanceRunning;
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = VMPaths.vmrunPath,
                        Arguments = $"GetGuestIPAddress \"{VMPaths.vmxPath}\"",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    };
                    using (Process process = Process.Start(startInfo))
                    {
                        string output = process.StandardOutput.ReadToEnd();
                        //sometimes if the machine is not on, it return 255.255.255.255 which is equal to broadcast. 
                        if (IPAddress.TryParse(output.Trim(), out var ip) && !ip.Equals(IPAddress.Broadcast))
                        {
                            _isVMwareInstanceRunning = true;
                        }
                        else
                        {
                            _isVMwareInstanceRunning = false;
                        }
                    }
                    if (_isVMwareInstanceRunning != previousStatus)
                    {
                        VMStatusChanged?.Invoke(this, _isVMwareInstanceRunning);
                    }
                    try
                    {
                        await Task.Delay(HeartbeatInterval, token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            }, token);
        }
        public void StopVMwareInstance()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = VMPaths.vmrunPath,
                Arguments = $"stop \"{VMPaths.vmxPath}\" soft",
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };
            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();

                process.WaitForExit();
            }
            _isVMwareInstanceRunning = false;
            VMStatusChanged?.Invoke(this, _isVMwareInstanceRunning);

        }
    }
}
