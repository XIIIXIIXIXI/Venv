﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Venv.Models.Interfaces;

namespace Venv.Models.DockerHandler
{
    public class SshClient : ISshClient
    {
        private readonly IPAddress _host;
        private readonly string _username;
        private Process _sshProcess;
        private bool _isConnected;

        public SshClient(IPAddress host, string username)
        {
            _host = host;
            _username = username;
            _isConnected = false;
        }
        public bool IsConnected => _isConnected;

        public void Connect()
        {
            if (!_isConnected)
            {
                _sshProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c ssh -tt {_username}@{_host} ",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
                };
                _sshProcess.Start();
                //TODO check if connection valid
                _isConnected = true;
                var trash = GetStandardOutput();
            }
            else
            {
                throw new InvalidOperationException("SSH client is already connected");
            }
        }
        public async Task DisconnectAsync()
        {
            if (_isConnected && _sshProcess != null && !_sshProcess.HasExited)
            {
                _sshProcess.StandardInput.WriteLine("exit");
                await _sshProcess.WaitForExitAsync();
                _isConnected = false;
            }
        }

        public void ExecuteCommand(string command)
        {
            if (_isConnected && _sshProcess != null && !_sshProcess.HasExited)
            {
                _sshProcess.StandardInput.Write(command + "\n");
                _sshProcess.StandardInput.Flush();
            }
            else
            {
                throw new InvalidOperationException("SSH client is not connected.");
            }
        }
        public StreamReader GetStandardOutput()
        {
            if (_isConnected && _sshProcess != null && !_sshProcess.HasExited)
            {
                return _sshProcess.StandardOutput;
            }
            else
            {
                throw new InvalidOperationException("SSH client is not connected or process is not running.");
            }
        }
    }
}
