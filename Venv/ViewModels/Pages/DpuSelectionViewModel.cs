using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Venv.Services;
using CommunityToolkit.Mvvm.Input;

namespace Venv.ViewModels.Pages
{
    public partial class DpuSelectionViewModel : ObservableObject
    {

        private readonly ShipDataService _shipDataService;
        [ObservableProperty]
        private IEnumerable<MachineryGroup> _machineryGroups;
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
                new MachineryGroup {Number = 1, Name = "Power", DPUs = group1},
                new MachineryGroup { Number = 2, Name = "Water", DPUs = group2 },
                new MachineryGroup {Number = 3, Name = "RPM", DPUs = group3},
                new MachineryGroup { Number = 4, Name = "Sonar Electricity", DPUs = group4 },
                new MachineryGroup {Number = 5, Name = "Water water with water", DPUs = group5},
                new MachineryGroup { Number = 6, Name = "Ship boat ship", DPUs = group6 },
                new MachineryGroup { Number = 7, Name = "ding dong ding", DPUs = group7 }
            };
            MachineryGroups = testMachineryGroupList;

        }
        public List<DPU> DpuList => _shipDataService.GetDpus();

        public void UpdateDpuSelectionBasedOnGroups(List<MachineryGroup> selectedGroups)
        {
            var allDpus = DpuList;
            foreach (var dpu in allDpus)
            {
                dpu.IsSelected = selectedGroups.Any(group => group.DPUs.Contains(dpu));
            }
            OnPropertyChanged(nameof(DpuList));
        }
    }
}
