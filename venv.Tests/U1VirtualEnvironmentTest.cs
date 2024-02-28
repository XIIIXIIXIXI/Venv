using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace venv.Tests
{
    [TestClass]
    public class U1VirtualEnvironmentTest
    {
        //If the vm returns an IP after starting then the VM has started successfully
        [TestMethod, Timeout(80000)] //This test can absolute maximum run for 1 minute and 20 seconds. It normally takes 45 seconds. 
        public void StartsVmwareProcessInHeadlessMode()
        {
            var _VMwareManager = new VMwareManager();
            var ipHost = _VMwareManager.StartVMwareInstance();
            Assert.IsNotNull(ipHost);
        }
    }
}
