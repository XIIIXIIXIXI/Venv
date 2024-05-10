using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using Venv.Models;
using Venv.Models.Interfaces;

namespace Venv.ViewModels.Pages
{

    public partial class DpuSelectionViewModel : ObservableObject
    {

        private readonly IShipDataService _shipDataService;
        [ObservableProperty]
        private IEnumerable<MachineryGroup> _machineryGroups;
        [ObservableProperty]
        private bool _isChecked =true;
        private readonly IDispatcherQueue _dispatcherQueue;

        private bool _ignoreSelectAllTrigger = false;

        public DpuSelectionViewModel(IShipDataService shipDataService, IDispatcherQueue dispatcherQueue)
        {
            _shipDataService = shipDataService;
            MachineryGroups = _shipDataService.MachineryGroup;
            _shipDataService.DataUpdated += RefreshData;
            _dispatcherQueue = dispatcherQueue;
        }

        public List<DPU> DpuList => _shipDataService.GetDpus();

        public void UpdateDpuSelectionBasedOnGroups(List<MachineryGroup> selectedGroups)
        {
            _ignoreSelectAllTrigger = true;
            var allDpus = DpuList;
            foreach (var dpu in allDpus)
            {
                bool isSelected = selectedGroups.Any(group => group.DPUs.Contains(dpu));
                dpu.IsSelected = isSelected;
            }

            IsChecked = false; 
            OnPropertyChanged(nameof(DpuList));
            _ignoreSelectAllTrigger = false;
        }

        public void SelectAllDPUs(bool isSelected)
        {
            if (_ignoreSelectAllTrigger)
            {
                return;
            }
            foreach (var dpu in DpuList)
            {
                dpu.IsSelected = isSelected;
            }
            OnPropertyChanged(nameof(DpuList));
        }
        public void RefreshData()
        {
            MachineryGroups = _shipDataService.MachineryGroup;
            SelectAllDPUs(true);
            IsChecked = true;
            _dispatcherQueue.TryEnqueue(() =>
            {
                OnPropertyChanged(nameof(DpuList));
            });
            
        }
    }
}
