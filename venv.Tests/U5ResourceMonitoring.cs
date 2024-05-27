using Microsoft.UI.Dispatching;
using Moq;
using Venv.Models.Data_Access;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.Resources;
using Venv.ViewModels.Pages;

namespace venv.Tests
{
    [TestClass]
    public class U5ResourceMonitoring
    {
        private IVMwareManager _vmwareManager;
        private Mock<IDispatcherQueue> _mockDispatcherQueue;
        private MonitoringViewModel _monitoringViewModel;

        //If the vm returns an IP after starting then the VM has started successfully
        //This methode runs everytime between each test methode.
        [TestInitialize, Timeout(120000)]//The setup can absolute maximum run for 2 minutes. It normally takes 1 minute and 30 seconds to start the VM.
        public void ClassSetup()
        {
            
        }
        [TestMethod]
        public async Task LoadPerformanceDataAsync_UpdatedProperties()
        {
            if (_vmwareManager == null)
            {
                _vmwareManager = new VMwareManager();
            }

            if (!_vmwareManager.IsVMwareInstanceRunning)
            {
                _vmwareManager.StartVMwareInstance();
            }
            Assert.IsTrue(_vmwareManager.IsVMwareInstanceRunning);

            //Arrange
            _mockDispatcherQueue = new Mock<IDispatcherQueue>();
            _mockDispatcherQueue.Setup(x => x.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
        .Callback<DispatcherQueueHandler>(action => action());

            _monitoringViewModel = new MonitoringViewModel(_vmwareManager, _mockDispatcherQueue.Object);

            await Task.Delay(2000);
            Assert.IsTrue(_monitoringViewModel.CpuUsage > 0, "expected CPU usage to be above 0");
            Assert.IsFalse(string.IsNullOrEmpty(_monitoringViewModel.CpuUsageText), "Expected 'CpuUsageText' to be not empty");
            Assert.IsTrue(_monitoringViewModel.MemoryUsagePercentage > 0, "Expected Memory % to be above 0");
            Assert.IsFalse(string.IsNullOrEmpty(_monitoringViewModel.MemoryUsageText), "Expected 'MemoryUsageText' to be not empty");
            Assert.IsTrue(_monitoringViewModel.CpuUsagePoints.Count > 0, "Data points not loaded");

        }

        [TestMethod]
        public void ReadFromVMXFile_Get_totalProcesses_VmCurrentProcesses()
        {
            VmxConfig config = new VmxConfig();


            (var hostTotalProcessors, var VMCurrentProcessors ) = config.ReadVmxConfiguration(VMPaths.vmxPath);

            Assert.AreEqual(16, hostTotalProcessors);
            Assert.AreEqual(4, VMCurrentProcessors);
        }

        //This test assumes the ReadFromVmxFileIsWorking properly
        [TestMethod]
        public void UpdateProcessorsInVmxFile()
        {
            var config = new VmxConfig();

            config.UpdateVmxProcessors(VMPaths.vmxPath, 6);

            Assert.AreEqual((16,6), config.ReadVmxConfiguration(VMPaths.vmxPath));
        }
    }
}
