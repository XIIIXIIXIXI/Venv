using System;
using System.Collections.Generic;

namespace Venv.Models.Interfaces
{
    public interface IShipDataService
    {
        List<DPU> DPUs { get; set; }
        string DatabaseVersion { get; }
        string DPUVersion { get; }
        int NumberOfMFD { get; }
        string VesselName { get; }
        string IMO { get; }
        string YardBuildNumber { get; }
        int SequenceNumber { get; }
        string GenerationDate { get; }
        string ShipOwner { get; }
        string Yard { get; }
        string FicVersion { get; }
        int SwitchesNumber { get; }
        int ShipType { get; }
        bool IsVirtualizationStopping { get; set; }
        List<MachineryGroup> MachineryGroup { get; set; }

        event Action DataUpdated;

        void UpdateShipData(string databaseVersion, string dpuVersion, int numberOfMfd, string vesselName, string imo, List<DPU> dpus,
                            List<MachineryGroup> machineryGroups, string yardBuildNumber, int sequenceNumber, string yardName, string ficVersion,
                            int switchesNumber, string shipOwner, int shipType, string generationDate);

        void UpdateDpuStatus(int dpuNumber, string status);
        List<DPU> GetDpus();
        bool IsDpuInFinalState(DPU singleDpu);
        List<DPU> GetSelectedDpus();
        bool AreAllDpusInFinalState();
        bool AnyDpuInState(string status);
    }
}
