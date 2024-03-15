using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Models.DockerHandler.Interfaces
{
    public interface ISshClient
    {
        // Establishes an SSH connection
        public void Connect();

        // Closes the SSH connection
        Task DisconnectAsync();

        // Executes a command over SSH
        public void ExecuteCommand(string command);

        public StreamReader GetStandardOutput();

        // Checks if the SSH client is connected
        bool IsConnected { get; }
    }
}
