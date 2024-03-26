﻿using CommunityToolkit.Mvvm.ComponentModel;
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
            MachineryGroups = _shipDataService.MachineryGroup;
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
