using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models.Network;

namespace venv.Tests
{
    [TestClass]
    public class U7EnvironmentAndConfiguration
    {
        private NetworkAdapterManager _networkAdapterManager;

        [TestInitialize]
        public void Setup()
        {
            _networkAdapterManager = new NetworkAdapterManager();
        }

        [TestMethod]
        public void CheckAdapterConfiguration()
        {
            var result = _networkAdapterManager.CheckConfiguration();
            Assert.IsTrue(result, "fail");
        }
    }
}
