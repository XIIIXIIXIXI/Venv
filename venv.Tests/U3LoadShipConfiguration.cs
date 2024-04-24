using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Newtonsoft.Json;
using Venv.Models.Services;
using Venv.Resources;

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
            string folderPath = VMPaths.confTestPath;
            _dataExtractor.SetFolderPath(folderPath);
        }
        /*
        [TestMethod]
        public void LoadDPUTest()
        {
            List<DPU> expectedDPUs = new List<DPU>
            {
                new DPU {Number = 1, Status="Off" },
                new DPU {Number = 11, Status="Off" },
                new DPU {Number = 12, Status="Off"},
                new DPU {Number = 22, Status="Off"},
                new DPU {Number = 31, Status="Off"},
                new DPU {Number = 41, Status="Off"},
                new DPU {Number = 51, Status="Off"},
                new DPU {Number = 52, Status="Off"},
                new DPU {Number = 61, Status="Off"},
                new DPU {Number = 62, Status="Off"},
                new DPU {Number = 71, Status="Off"},
            };

            
            var actualDPUs = _dataExtractor.ExtractDpus();

            Assert.AreEqual(expectedDPUs.Count, actualDPUs.Count, "The number of extracted DPUs does not match the expected count.");

            string expectedJson = JsonConvert.SerializeObject(expectedDPUs, Formatting.Indented);
            string actualJson = JsonConvert.SerializeObject(actualDPUs, Formatting.Indented);

            //Assert.AreEqual(expectedJson, actualJson, "The extracted DPUs do not match the expected.");
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
            Assert.AreEqual("2.1.16.05", result);
        }
        [TestMethod]
        public void GetMFDsAmount_ReturnsCorrectAmount()
        {
            var result = _dataExtractor.GetMFDsAmount();
            Assert.AreEqual(6, result);
        }
        [TestMethod]
        public void GetVesselName_ReturnsCorrectName()
        {
            var result = _dataExtractor.GetVesselName();
            Assert.AreEqual("Dalian P110K", result);
        }
        [TestMethod]
        public void GetIMONumber_ReturnsCorrectNumber()
        {
            var result = _dataExtractor.GetIMONumber();
            Assert.AreEqual("?", result);
        }
        [TestMethod]
        public void CreateShipData_Success()
        {   // Work pc
            //ShipConfigurationFactory factory = new ShipConfigurationFactory(@"C:\IM_DBs\Demo config 2.1.16.10 Seq 470 AUT NAV\Demo config 2.1.16.10 Seq 470 AUT NAV");
            // Home pc
            ShipConfigurationFactory factory = new ShipConfigurationFactory(VMPaths.confTestPath);
            ShipDataService shipData = factory.Create();
            Assert.AreEqual("2161v2", shipData.DatabaseVersion);
            Assert.AreEqual("2.1.16.05", shipData.DPUVersion);
            Assert.AreEqual(6, shipData.NumberOfMFD);
            Assert.AreEqual("Dalian P110K", shipData.VesselName);
            Assert.AreEqual("?", shipData.IMO);
            Assert.AreEqual("P110K", shipData.YardBuildNumber);
            Assert.AreEqual(283, shipData.SequenceNumber);
            Assert.AreEqual("Dalian", shipData.Yard);
            Assert.AreEqual("2.1.16.05", shipData.FicVersion);
            Assert.AreEqual(20, shipData.SwitchesNumber);
        }
        [TestMethod]
        public void CreateShipData_Failure()
        {

        }
    }
}