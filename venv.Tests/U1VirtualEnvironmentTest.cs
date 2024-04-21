using Microsoft.UI.Dispatching;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using Venv.Models.DockerHandler;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.ViewModels.Pages;


namespace venv.Tests
{
    [TestClass]
    public class U1VirtualEnvironmentTest
    {
        private VMwareManager _vmwareManager;
        private VirtualViewModel _virtualViewModel;

        
        //If the vm returns an IP after starting then the VM has started successfully
        //This methode runs everytime between each test methode.
        [TestInitialize, Timeout(120000)]//This test can absolute maximum run for 2 minutes. It normally takes 1 minute and 30 seconds.
        public void ClassSetup()
        {
            if (_vmwareManager == null)
            {
                _vmwareManager = new VMwareManager();
                
            }
            
            if (!_vmwareManager.IsVMwareInstanceRunning)
            {
                _vmwareManager = new VMwareManager();
                _vmwareManager.StartVMwareInstance();
            }
            Assert.IsTrue(_vmwareManager.IsVMwareInstanceRunning);

            
        }
        
        /*
         * This test methode tests many things because otherwise the VM has to start and stop constantly which takes too much time
         * 
         * Tests if the VM is sending the right signal when it has started and when it has stopped.
         * Tests if the Viewmodel gets updated when the VM changes its status.
         */
        [TestMethod]
        public async Task VMStatusMonitoring_VirtualizationPageSpinner()
        {
            var mockService = new Mock<ShipDataService>();
            var mockDispatcherQueue = new Mock<IDispatcherQueue>();
            mockDispatcherQueue.Setup(x => x.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
        .Callback<DispatcherQueueHandler>(action => action());

            _virtualViewModel = new VirtualViewModel(_vmwareManager, mockService.Object, null, mockDispatcherQueue.Object);

            _vmwareManager.StopVMwareInstance();
            _vmwareManager.StartHeartBeat();
            Assert.IsFalse(_vmwareManager.IsVMwareInstanceRunning);

            //Check if the viewmodel gets updated when status is changed in VM
            Assert.IsFalse(_virtualViewModel.IsVMRunning);
            _vmwareManager.StartVMwareInstance();
            await Task.Delay(_vmwareManager.HeartbeatInterval + 1000); // to make sure the output is from the heartbeat and not from starting the vmwareInstance.
            Assert.IsTrue(_vmwareManager.IsVMwareInstanceRunning);
            Assert.IsTrue(_virtualViewModel.IsVMRunning);
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
