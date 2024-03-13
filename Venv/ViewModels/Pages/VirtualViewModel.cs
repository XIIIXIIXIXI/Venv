using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Dispatching;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Venv.Models;
using Venv.Models.DockerHandler;
using Venv.Services;


namespace Venv.ViewModels.Pages
{
    public partial class VirtualViewModel : ObservableObject
    {
        public ObservableCollection<DPU> Dpus { get; } = new();
        // it is only possible to update the UI with the main thread in WinUi that is why we need the dispatcher queue. 
        private readonly DispatcherQueue _dispatcherQueue;

        private readonly VMwareManager _vmwareManager;
        private readonly ShipDataService _shipDataService;
        public VirtualViewModel(VMwareManager vmwareManager, ShipDataService shipDataService)
        {
            _shipDataService = shipDataService;
            _vmwareManager = vmwareManager;
            _shipDataService.DataUpdated += OnDataUpdated;
            _vmwareManager.VMStatusChanged += OnVMStatusChanged;
            //_dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            //_ = StartVMasync();
            //_vmwareManager.StartHeartBeat();
        }

        [ObservableProperty]
        private bool isVMRunning;

        public List<DPU> DpuList => _shipDataService.GetDpus();

        private void OnDataUpdated()
        {
            OnPropertyChanged(nameof(DpuList));
        }

        private void OnVMStatusChanged(object sender, bool isRunning)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsVMRunning = isRunning;
            });         
        }
        private async Task StartVMasync()
        {
            await Task.Run(() =>
            {
                _vmwareManager.StartVMwareInstance();
            });
        }
    }
}

