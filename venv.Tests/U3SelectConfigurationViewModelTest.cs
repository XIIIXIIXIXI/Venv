using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.Resources;
using Venv.Services;
using Venv.ViewModels.Pages;

namespace venv.Tests
{
    [TestClass]
    public class U3SelectConfigurationViewModelTest
    {
        private Mock<IWindowHandleProvider> _mockWindowHandleProvider;
        private Mock<ShipDataService> _mockShipDataService;
        private Mock<IVMwareManager> _mockVMwareManager;
        private SelectConfigurationViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _mockWindowHandleProvider = new Mock<IWindowHandleProvider>();
            _mockShipDataService = new Mock<ShipDataService>();
            _mockVMwareManager = new Mock<IVMwareManager>();
            _viewModel = new SelectConfigurationViewModel(_mockWindowHandleProvider.Object, _mockShipDataService.Object, _mockVMwareManager.Object);
        }
        [TestMethod]
        public void SelectFolderAsync_ValidFolder_SetsShipDataAndUpdatesRecentConfiguration()
        {
            // Arrange
            string validPath = VMPaths.confTestPath;
            _viewModel.LoadConfigurationCommand.Execute(new ConfigurationModel { FilePath = validPath });

            // Act
            Assert.IsTrue(_viewModel.IsConfigurationSelected);
            _mockShipDataService.Verify(x => x.UpdateShipData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<DPU>>(), It.IsAny<List<MachineryGroup>>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
        }
        [TestMethod]
        public void ShouldUpdateViewModelWhenShipDataServiceChanges()
        {
            var newDpus = new List<DPU> { new DPU { Number = 1, Status = "Running" } };
            _mockShipDataService.Setup(s => s.GetDpus()).Returns(newDpus);

            _mockShipDataService.Raise(s => s.DataUpdated += null, EventArgs.Empty);

            Assert.AreEqual(_viewModel.ShipData.DPUs[0], newDpus);
        }
    }
}
