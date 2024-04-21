using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.ViewModels.Pages;

namespace venv.Tests
{
    /*
    [TestClass]
    public class U5MonitoringViewModelTest
    {
        private Mock<IVMwareManager> _vmwareManagerMock;
        private MonitoringViewModel _viewModel;
        private VmMonitoringService _monitoringService;
        private IPAddress vmIP = IPAddress.Parse("192.168.1.100"); // Example IP

        [TestInitialize]
        public void Setup()
        {
            _vmwareManagerMock = new Mock<IVMwareManager>();
            _vmwareManagerMock.SetupGet(vm => vm.IP).Returns(vmIP);

            _monitoringService = new VmMonitoringService(vmIP);
            _viewModel = new MonitoringViewModel(_vmwareManagerMock.Object);
        }
    }*/
}
