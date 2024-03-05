using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Newtonsoft.Json;

namespace venv.Tests
{
    [TestClass]
    public class U3LoadShipConfiguration
    {
        private XmlDataExtractor _dataExtractor;
        [TestInitialize]
        public void Setup()
        {
            _dataExtractor = new XmlDataExtractor();
            string folderPath = @"C:\IM_DBs\Demo config 2.1.16.10 Seq 470 AUT NAV\Demo config 2.1.16.10 Seq 470 AUT NAV\XML";
            _dataExtractor.SetFolderPath(folderPath);
        }
        [TestMethod]
        public void LoadDPUTest()
        {
            List<DPU> expectedDPUs = new List<DPU>
            {
                new DPU {Number = 1, Status="Off"},
                new DPU {Number = 2, Status="Off" },
                new DPU {Number = 3, Status="Off"},
                new DPU {Number = 4, Status="Off"},
            };

            
            var actualDPUs = _dataExtractor.ExtractDpus();

            Assert.AreEqual(expectedDPUs.Count, actualDPUs.Count, "The number of extracted DPUs does not match the expected count.");

            string expectedJson = JsonConvert.SerializeObject(expectedDPUs, Formatting.Indented);
            string actualJson = JsonConvert.SerializeObject(actualDPUs, Formatting.Indented);

            // Assert
            Assert.AreEqual(expectedJson, actualJson, "The extracted DPUs do not match the expected.");
        }
        [TestMethod]
        public void GetDatabaseVersion_ReturnsCorrectVersion()
        {
            var result = _dataExtractor.GetDatabaseVersion();
            Assert.AreEqual("2161v2", result);

        }
        [TestMethod]
        public void GetDPU2010Version_ReturnsCorrectVersion()
        {
            var result = _dataExtractor.GetDPU2010Version();
            Assert.AreEqual("2.1.16.04", result);
        }
        [TestMethod]
        public void GetMFDsAmount_ReturnsCorrectAmount()
        {
            var result = _dataExtractor.GetMFDsAmount();
            Assert.AreEqual(4, result);
        }
        [TestMethod]
        public void GetVesselName_ReturnsCorrectName()
        {
            var result = _dataExtractor.GetVesselName();
            Assert.AreEqual("Demo System", result);
        }
        [TestMethod]
        public void GetIMONumber_ReturnsCorrectNumber()
        {
            var result = _dataExtractor.GetIMONumber();
            Assert.AreEqual("123456", result);
        }
    }
}
