using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Newtonsoft.Json;
using Venv.Services;

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
            string folderPath = @"C:\IM_DBs\Demo config 2.1.16.10 Seq 470 AUT NAV\Demo config 2.1.16.10 Seq 470 AUT NAV";
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
        [TestMethod]
        public void CreateShipData()
        {   // Work pc
            //ShipConfigurationFactory factory = new ShipConfigurationFactory(@"C:\IM_DBs\Demo config 2.1.16.10 Seq 470 AUT NAV\Demo config 2.1.16.10 Seq 470 AUT NAV");
            // Home pc
            ShipConfigurationFactory factory = new ShipConfigurationFactory(@"C:\IM_DBs\MaerskTank - 2.1.16.06");
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
    }
}
/*
        public DpuSelectionViewModel(ShipDataService shipDataService)
        {
            
            _shipDataService = shipDataService;
            //populate shipdata for testing
            List<DPU> testList = new List<DPU>
            {
                new DPU {Number = 1, Status="Off", IsSelected = true},
                new DPU {Number = 2, Status="Off", IsSelected = true },
                new DPU {Number = 3, Status="Off", IsSelected = true},
                new DPU {Number = 4, Status="Off", IsSelected = true},
                new DPU {Number = 5, Status="Off", IsSelected = true},
                new DPU {Number = 6, Status="Off", IsSelected = true },
                new DPU {Number = 7, Status="Off", IsSelected = true},
                new DPU {Number = 8, Status="Off", IsSelected = true},
                new DPU {Number = 9, Status="Off", IsSelected = true},
                new DPU {Number = 10, Status="Off", IsSelected = true },
                new DPU {Number = 11, Status="Off", IsSelected = true},
                new DPU {Number = 12, Status="Off", IsSelected = true},
            };
            _shipDataService.UpdateShipData("1", "11", 1, "testvessel", "1", testList);

            List<DPU> group1 = new List<DPU>
            {
                _shipDataService.DPUs[0],
                _shipDataService.DPUs[1]
            };
            List<DPU> group2 = new List<DPU>
            {
                _shipDataService.DPUs[2],
                _shipDataService.DPUs[3]
            };
            List<DPU> group3 = new List<DPU>
            {
                _shipDataService.DPUs[1],
                _shipDataService.DPUs[2]
            };
            List<DPU> group4 = new List<DPU>
            {
                _shipDataService.DPUs[2],
                _shipDataService.DPUs[3],
                _shipDataService.DPUs[1],
                _shipDataService.DPUs[6],
                _shipDataService.DPUs[10],
                _shipDataService.DPUs[11]
            };
            List<DPU> group5 = new List<DPU>
            {
            };
            List<DPU> group6 = new List<DPU>
            {
                _shipDataService.DPUs[2],
                _shipDataService.DPUs[3],
                _shipDataService.DPUs[1],
                _shipDataService.DPUs[8],
                _shipDataService.DPUs[7],
                _shipDataService.DPUs[5]
            };
            List<DPU> group7 = new List<DPU>
            {
                _shipDataService.DPUs[2],
                _shipDataService.DPUs[3]
            };
            List<MachineryGroup> testMachineryGroupList = new List<MachineryGroup>
            {
                new MachineryGroup {Number = 1, Name = "Not used", DPUs = group1},
                new MachineryGroup { Number = 2, Name = "ECR", DPUs = group2 },
                new MachineryGroup {Number = 3, Name = "Cargo", DPUs = group3},
                new MachineryGroup { Number = 4, Name = "PMS", DPUs = group4 },
                new MachineryGroup {Number = 5, Name = "Cooling Sea Water", DPUs = group5},
                new MachineryGroup { Number = 6, Name = "EPS", DPUs = group6 },
                new MachineryGroup { Number = 7, Name = "Chinese Spy Equipment", DPUs = group7 }
            };
            MachineryGroups = testMachineryGroupList;

        }*/