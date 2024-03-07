using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;

namespace Venv.Services
{
    public class ShipDataService
    {
        public List<DPU> DPUs { get; private set; }
        public string DatabaseVersion { get; private set; }
        public string DPUVersion { get; private set; }
        public int NumberOfMFD { get; private set; }
        public string VesselName { get; private set; }
        public string IMO { get; private set; }
        public event Action DataUpdated;

        public ShipDataService(string databaseVersion, string dpuVersion, int numberOfMfd, string vesselName, string imo, List<DPU> dpus)
        {
            DatabaseVersion = databaseVersion;
            DPUVersion = dpuVersion;
            NumberOfMFD = numberOfMfd;
            VesselName = vesselName;
            IMO = imo;
            DPUs = dpus;
        }

        public void UpdateDpuStatus(int dpuNumber, string status)
        {
            var dpu = DPUs.Find(d => d.Number == dpuNumber);
            if (dpu != null)
            {
                dpu.Status = status;
                DataUpdated?.Invoke();
            }
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
            var dpus = _dataExtractor.ExtractDpus();
            var databaseVersion = _dataExtractor.GetDatabaseVersion();
            var dpuVersion = _dataExtractor.GetDPU2010Version();
            var numberOfMFD = _dataExtractor.GetMFDsAmount();
            var vesselName = _dataExtractor.GetVesselName();
            var imo = _dataExtractor.GetIMONumber();

            return new ShipDataService(databaseVersion, dpuVersion, numberOfMFD, vesselName, imo, dpus);
        }
    }
}
