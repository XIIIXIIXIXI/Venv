using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Venv.Models.DockerHandler.Interfaces;
using Venv.Resources;

namespace Venv.Models.DockerHandler
{
    public class VMwareManager : IVMwareManager
    {
        private bool _isVMwareInstanceRunning { get; set; }
        public bool IsVMwareInstanceRunning => _isVMwareInstanceRunning;

        public VMwareManager() 
        {
            _isVMwareInstanceRunning = false;
        }

        public Task ExecuteVMwareCommandAsync(string command)
        {
            throw new NotImplementedException();
        }

        public IPAddress StartVMwareInstance()
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
                return WaitingForVMToBeTurnedOn();
            }
            else
            {
                throw new InvalidOperationException("VMware instance is already running.");
            }
        }

        private IPAddress WaitingForVMToBeTurnedOn()
        {
            const int maxRetries = 60; // Maximum number of retries
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
                        _isVMwareInstanceRunning = true;
                        return ip;
                    }
                }
            }
            return IPAddress.None;
        }
        public Task StopVMwareInstanceAsync()
        {
            throw new NotImplementedException();
        }
    }
}
