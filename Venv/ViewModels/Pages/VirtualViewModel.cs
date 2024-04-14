﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Venv.Models;
using Venv.Models.Services;


namespace Venv.ViewModels.Pages
{
    public partial class VirtualViewModel : ObservableObject
    {
        public ObservableCollection<DPU> Dpus { get; } = new();

        // it is only possible to update the UI with the main thread in WinUi that is why we need the dispatcher queue. 
        private readonly DispatcherQueue _dispatcherQueue;

        private readonly VMwareManager _vmwareManager;
        private readonly ShipDataService _shipDataService;
        private readonly Mediator _mediator;
        public VirtualViewModel(VMwareManager vmwareManager, ShipDataService shipDataService, Mediator mediator)
        {
            _shipDataService = shipDataService;
            _vmwareManager = vmwareManager;
            _mediator = mediator;
            _shipDataService.DataUpdated += OnDataUpdated;
            _vmwareManager.VMStatusChanged += OnVMStatusChanged;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            IsVMRunning = _vmwareManager.IsVMwareInstanceRunning;
            _vmwareManager.StartHeartBeat();
        }

        [ObservableProperty]
        private bool isVMRunning;
        [ObservableProperty]
        private string _buttonText = "Start Virtualization";
        [ObservableProperty]
        private bool isButtonEnabled = true;
        //For gridview
        [ObservableProperty]
        private DPU _selectedDpu;

        //notify navigation
        public event Action<bool> VirtualizationRunningChanged;
        [ObservableProperty]
        private bool isVirtualizationRunning = false;
        partial void OnIsVirtualizationRunningChanged(bool value)
        {
            VirtualizationRunningChanged?.Invoke(value);
        }

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
        
        [RelayCommand]
        public async Task OnStartStopVirtualization()
        {
            IsButtonEnabled = false;
            
            try
            {
                if (ButtonText == "Start Virtualization")
                {
                    ButtonText = "Starting Containers..";
                    IsVirtualizationRunning = true;
                    _shipDataService.IsVirtualizationStopping = false;
                    await _mediator.StartDockerContainersAsync();
                }
                else if (ButtonText == "Stop Virtualization")
                {
                    ButtonText = "Stopping Containers..";
                    IsVirtualizationRunning = false;
                    _shipDataService.IsVirtualizationStopping = true;
                    await _mediator.StopDockerContainersAsync();
                }
            }
            catch
            {
                //ignore
            }
            UpdateButtonState();
        }
        private void UpdateButtonState()
        {
            if (_shipDataService.AreAllDpusInFinalState())
            {
                ButtonText = _shipDataService.AnyDpuInState("Removed") ? "Start Virtualization" : "Stop Virtualization";
                IsButtonEnabled = true;
            }
            else
            {
                ButtonText = "Loading";
                IsButtonEnabled = false;
            }
        }
    }
}

