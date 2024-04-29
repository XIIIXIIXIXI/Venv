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
using Microsoft.UI.Xaml.Controls;

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
            _dispatcherQueue.TryEnqueue(() =>
            {
                OnPropertyChanged(nameof(DpuList));
            });
            
        }
    }
}
