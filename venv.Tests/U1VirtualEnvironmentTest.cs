using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Venv.Models.DockerHandler;


namespace venv.Tests
{
    [TestClass]
    public class U1VirtualEnvironmentTest
    {
        //If the vm returns an IP after starting then the VM has started successfully
        [TestMethod, Timeout(8000000)] //This test can absolute maximum run for 2 minutes. It normally takes 1 minute and 30 seconds. 
        public void StartsVmwareProcessInHeadlessMode()
        {
            var _VMwareManager = new VMwareManager();
            var ipHost = _VMwareManager.StartVMwareInstance();
            Assert.AreNotEqual(IPAddress.None, ipHost);
        }

        [TestMethod]
        public async Task VMStatusMonitoring()
        {
            var _VMwareManager = new VMwareManager();
            _VMwareManager.StopVMwareInstance();
            _VMwareManager.StartHeartBeat();
            Assert.IsFalse(_VMwareManager.IsVMwareInstanceRunning);
            _VMwareManager.StartVMwareInstance();
            await Task.Delay(_VMwareManager.HeartbeatInterval + 1000); // to make sure the output is from the heartbeat and not from starting the vmwareInstance.
            Assert.IsTrue(_VMwareManager.IsVMwareInstanceRunning);
        }


        [TestClass]
        public class GeneralTests
        {
            [TestMethod]
            [Ignore("Performance testing")]
            public void StartVM10TimesAndTimeEach()
            {
                var _VmwareManager = new VMwareManager();
                List<long> timings = new List<long>();
                _VmwareManager.StopVMwareInstance();
                var stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < 4; i++)
                {
                    stopwatch.Start();
                    _VmwareManager.StartVMwareInstance();
                    stopwatch.Stop();
                    timings.Add(stopwatch.ElapsedMilliseconds / 1000);
                    _VmwareManager.StopVMwareInstance();
                    stopwatch.Reset();
                }

                var averageTime = timings.Average();
                Console.WriteLine("Timings (ms): " + string.Join(", ", timings));
                Console.WriteLine("Average Start Time (ms): " + averageTime);
            }
        }
        
    }
}
