using System.Net;
using Venv.Models.Services;

namespace venv.Tests
{
    [TestClass]
    public class U7EnvironmentAndConfiguration
    {
        private NetworkAdapterManager _networkAdapterManager;
        private VmNetworkManager _vmNetworkManager;

        [TestInitialize]
        public void Setup()
        {
            _networkAdapterManager = new NetworkAdapterManager();
            IPAddress.TryParse("192.168.112.131", out var ip);
            _vmNetworkManager = new VmNetworkManager(ip);
        }

        [TestMethod]
        public void CheckAdapterConfiguration()
        {
            var result = _networkAdapterManager.CheckConfiguration();
            Assert.IsTrue(result, "fail");
        }
        [TestMethod]
        public void ConfigureAdapter()
        {
            _networkAdapterManager.Configure();

            Assert.IsTrue(_networkAdapterManager.CheckConfiguration(), "Adapter wrong IP");
        }

        /*
        [TestMethod]
        public void CheckVMNetworkConfiguration()
        {
            var result = _vmNetworkManager.CheckAndConfigureNetwork();

            Assert.IsFalse(result, "VM wrong IP but should be changed on the second run. If the error comes the second time then something is wrong with the logic");
        }*/
    }
}
