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
        /*
         * This test methode tests many things because otherwise the VM has to start and stop constantly which takes too much time
         * 
         * Objectives:
         * - Validate that the VM can start and stop as expected.
         * - Check the functionality of the heartbeat monitoring mechanism.
         * - Confirm that VM status updates are correctly transmitted and reflected in the ViewModel.
         */
        [TestMethod]
        public async Task VMStatusMonitoring_VirtualizationPageSpinner()
        {
            var mockService = new Mock<ShipDataService>();
            var mockDispatcherQueue = new Mock<IDispatcherQueue>();
            mockDispatcherQueue.Setup(x => x.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
        .Callback<DispatcherQueueHandler>(action => action());

            VMwareManager _vmwareManager = new VMwareManager();
            VirtualViewModel _virtualViewModel = new VirtualViewModel(_vmwareManager, mockService.Object, null, mockDispatcherQueue.Object);

            //stopping the VM
            _vmwareManager.StopVMwareInstance();
            Assert.IsFalse(_vmwareManager.IsVMwareInstanceRunning);

            // Start heartbeat and validate VM status update
            _vmwareManager.StartHeartBeat();
            Assert.IsFalse(_virtualViewModel.IsVMRunning);

            // starting the VM and ensure the ViewModel is updated after the heartbeat interval
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
