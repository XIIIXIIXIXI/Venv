using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Venv.Services;

namespace Venv.ViewModels.Pages
{
    public partial class DpuSelectionViewModel : ObservableObject
    {
        private readonly ShipDataService _shipDataService;
        DpuSelectionViewModel(ShipDataService shipDataService)
        {
            _shipDataService = shipDataService;
        }
        public List<DPU> DpuList => _shipDataService.GetDpus();
    }
}
