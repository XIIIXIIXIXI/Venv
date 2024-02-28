using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace venv.Tests
{
    [TestClass]
    public class U2LaunchingVirtualizationTests
    {
        private ISSHClientWrapper _sshClientWrapper;
        private VMwareManager _VMwareManager;
        private IPAddress host;
        [ClassInitialize]
        public static void Setup()
        {
            _VMwareManager = new VMwareManager();
            var ipHost = _VMwareManager.StartVMwareInstance();
        }
        [TestMethod]
        public void EstablishSshConnection()
        {
            host = _VMwareManager.GetHost();
            _sshClientWrapper = new ISSHClientWrapper(host, "vdpu");
            _sshClientWrapper.Connect();
            Assert.IsTrue(_sshClientWrapper.isConnected);
        }
        [TestMethod]
        public void SendConfigurationCommand_ExecuteBashScript()
        {
            var monitorTask = MonitorDockerEventsAsync();
            _sshClientWrapper.ExecuteCommand($"./ManageDockers.sh 2 3 4 5");
        }
        [TestMethod]
        public void MonitorDockerContainers()
        {

        }
        [TestMethod]
        public void VirtualizationNotReady()
        {

        }
        [TestMethod]
        public void SshConnectionFailure_InformUser()
        {

        }

    }

}
