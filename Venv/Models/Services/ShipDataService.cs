using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Venv.Models.Interfaces;

namespace Venv.Models.Services
{
    public class ShipDataService : IShipDataService
    {
        public List<DPU> DPUs { get; set; }
        public string DatabaseVersion { get; private set; }
        public string DPUVersion { get; private set; }
        public int NumberOfMFD { get; private set; }
        public string VesselName { get; private set; }
        public string IMO { get; private set; }
        //new
        public string YardBuildNumber { get; private set; }
        public int SequenceNumber { get; private set; }
        public string GenerationDate { get; private set; }
        public string ShipOwner { get; private set; }
        public string Yard { get; private set; }
        public string FicVersion { get; private set; }
        public int SwitchesNumber { get; private set; }
        public int ShipType { get; private set; }
        public event Action DataUpdated;
        public event Action NewShipConfiguration;
        public bool IsVirtualizationStopping { get; set; }

        public List<MachineryGroup> MachineryGroup { get; set; }

        public ShipDataService()
        {
        }


        public void UpdateShipData(
            string databaseVersion,
            string dpuVersion,
            int numberOfMfd,
            string vesselName,
            string imo, List<DPU> dpus,
            List<MachineryGroup> machineryGroups,
            string yardBuildNumber,
            int sequenceNumber,
            string yardName,
            string ficVersion,
            int switchesNumber,
            string shipOwner,
            int shipType,
            string generationDate
            )
        {
            DatabaseVersion = databaseVersion;
            DPUVersion = dpuVersion;
            NumberOfMFD = numberOfMfd;
            VesselName = vesselName;
            IMO = imo;
            DPUs = dpus;
            MachineryGroup = machineryGroups;
            YardBuildNumber = yardBuildNumber;
            SequenceNumber = sequenceNumber;
            Yard = yardName;
            FicVersion = ficVersion;
            SwitchesNumber = switchesNumber;
            ShipOwner = shipOwner;
            ShipType = shipType;
            GenerationDate = generationDate;
            NewShipConfiguration?.Invoke();
        }

        public void UpdateDpuStatus(int dpuNumber, string status)
        {
            var dpu = DPUs.Find(d => d.Number == dpuNumber);
            if (dpu != null)
            {
                dpu.StatusHolder = status;
                DataUpdated?.Invoke();
            }
        }
        public List<DPU> GetDpus()
        {
            return DPUs;
        }
        public bool IsDpuInFinalState(DPU singleDpu)
        {
            return singleDpu.Status == "Running" || singleDpu.Status == "Removed" || singleDpu.Status == "Started" || singleDpu.Status == "Stopped";
        }
        public List<DPU> GetSelectedDpus()
        {
            return DPUs.Where(x => x.IsSelected).ToList();
        }
        public bool AreAllDpusInFinalState()
        {
            if (IsVirtualizationStopping)
            {
                return GetSelectedDpus().TrueForAll(dpu => dpu.Status == "Removed");
            }
            else
            {
                return GetSelectedDpus().TrueForAll(dpu => dpu.Status == "Running" || dpu.Status == "Started");
            }
        }
        public bool AnyDpuInState(string status)
        {
            return GetSelectedDpus().Exists(dpu => dpu.Status == status);
        }

    }

    public class ShipConfigurationFactory
    {
        private readonly XmlDataExtractor _dataExtractor;

        public ShipConfigurationFactory(string filePath)
        {
            _dataExtractor = new XmlDataExtractor();
            _dataExtractor.SetFolderPath(filePath);
        }

        public ShipDataService Create()
        {
            if (!_dataExtractor.ValidateConfigurationFolder())
            {
                Debug.WriteLine("Xml file doesen't exist, make sure you choosed the right configuration");
                return null;
            }
            (var dpus, var machineryGroups) = _dataExtractor.AssociateDpusWithMachineryGorups();
            var databaseVersion = _dataExtractor.GetDatabaseVersion();
            var dpuVersion = _dataExtractor.GetDPU2010Version();
            var numberOfMFD = _dataExtractor.GetMFDsAmount();
            var vesselName = _dataExtractor.GetVesselName();
            var imo = _dataExtractor.GetIMONumber();
            var yardBuildNumber = _dataExtractor.GetYardBuildNumber();
            var sequenceNumber = _dataExtractor.GetSequenceNumber();
            var yardName = _dataExtractor.GetYardName();
            var ficVersion = _dataExtractor.GetFicVersion();
            var switchesNumber = _dataExtractor.GetSwitchesNumber();
            var shipOwner = _dataExtractor.GetShipOwner();
            var shipType = _dataExtractor.GetShipType();
            var generationDate = _dataExtractor.GetGenerationDate();

            var shipDataService = new ShipDataService();

            shipDataService.UpdateShipData(databaseVersion,
                dpuVersion, numberOfMFD,
                vesselName,
                imo, dpus,
                machineryGroups,
                yardBuildNumber,
                 sequenceNumber,
                 yardName,
                ficVersion,
                switchesNumber,
                shipOwner,
                shipType,
                generationDate);

            return shipDataService;
        }
    }
}
