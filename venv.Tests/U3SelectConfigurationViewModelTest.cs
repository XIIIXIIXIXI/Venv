using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models.Services;
using Venv.Services;
using Venv.ViewModels.Pages;

namespace venv.Tests
{
    [TestClass]
    public class U3SelectConfigurationViewModelTest
    {
        private SelectConfigurationViewModel _viewModel;
        private Mock<IWindowHandleProvider> _mockWindowHandleProvider;
        private Mock<ShipDataService> _mockShipDataService;
        private Mock<VMwareManager> _mockVMwareManager;
        [TestInitialize]
        public void Setup()
        {
            _mockWindowHandleProvider = new Mock<IWindowHandleProvider>();
            _mockShipDataService = new Mock<ShipDataService>();
            _mockVMwareManager = new Mock<VMwareManager>();
            _viewModel = new SelectConfigurationViewModel(_mockWindowHandleProvider.Object, _mockShipDataService.Object, _mockVMwareManager.Object);
        }
        [TestMethod]
        public void SelectFolderAsync_ValidFolder_UpdatesConfigurationSelected()
        {
            // Arrange
            var expectedFolderPath = @"C:\Valid\Path";
            _mockWindowHandleProvider.Setup(m => m.GetWindowHandle()).Returns(IntPtr.Zero);  // Assume GetWindowHandle works

            // Act
            _viewModel.SelectFolderCommand.Execute(null);

            // Assert
            Assert.IsTrue(_viewModel.IsConfigurationSelected, "Configuration should be selected for a valid folder.");
        }
    }
}
