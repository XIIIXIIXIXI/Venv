using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using CommunityToolkit.Mvvm.Input;
using Venv.Models.Services;
using Venv.Models.Interfaces;

namespace Venv.ViewModels.Pages
{

    public partial class DpuSelectionViewModel : ObservableObject
    {

        private readonly IShipDataService _shipDataService;
        [ObservableProperty]
        private IEnumerable<MachineryGroup> _machineryGroups;

        public DpuSelectionViewModel(IShipDataService shipDataService)
        {
            _shipDataService = shipDataService;
            MachineryGroups = _shipDataService.MachineryGroup;
            _shipDataService.DataUpdated += RefreshData;
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
        public void RefreshData()
        {
            MachineryGroups = _shipDataService.MachineryGroup;
            OnPropertyChanged(nameof(DpuList));
        }
    }
}
