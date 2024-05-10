using Microsoft.UI.Dispatching;
using Moq;
using Venv.Models;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.ViewModels.Pages;

namespace venv.Tests
{
    [TestClass]
    public class U2LaunchingVirtualization
    {
        private IVMwareManager _vmwareManager;
        private Mediator _mediator;
        private Mock<IDispatcherQueue> _mockDispatcherQueue;
        private  IShipDataService _shipDataService;
        private VirtualViewModel _viewModel;

        private int nDpusInTest = 4;
        [TestInitialize]
        public void Setup()
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

            _mockDispatcherQueue = new Mock<IDispatcherQueue>();
            _mockDispatcherQueue.Setup(x => x.TryEnqueue(It.IsAny<DispatcherQueueHandler>()))
        .Callback<DispatcherQueueHandler>(action => action());

            _shipDataService = new ShipDataService();
            var dpus = new List<DPU>();
            for (int i = 1; i <= nDpusInTest; i++)
            {
                dpus.Add(new DPU { Number = i, Status = "Stopped", IsSelected = true });
            }
            _shipDataService.DPUs = dpus;
            _mediator = new Mediator(_shipDataService, _vmwareManager);
            _viewModel = new VirtualViewModel(_vmwareManager, _shipDataService, _mediator, _mockDispatcherQueue.Object);

        }
        [TestMethod]
        public async Task LaunchVirtualizationTest()
        {
            await _viewModel.OnStartStopVirtualization();
            for (int i = 0; i < _shipDataService.DPUs.Count; i++)
            {
                Assert.AreEqual("Started", _shipDataService.DPUs[i].Status, $"DPU at index {i} is not in the 'Started' status.");
            }
            Assert.IsTrue(_viewModel.IsVirtualizationRunning);
            Assert.AreEqual(_viewModel.ButtonText, "Stop Virtualization");
            await _viewModel.OnStartStopVirtualization();
            for (int i = 0; i < _shipDataService.DPUs.Count; i++)
            {
                Assert.AreEqual("Removed", _shipDataService.DPUs[i].Status, $"DPU at index {i} is not in the 'Stopped' status.");
            }
            Assert.IsFalse(_viewModel.IsVirtualizationRunning);
            Assert.AreEqual(_viewModel.ButtonText, "Start Virtualization");
        }
    }
}
