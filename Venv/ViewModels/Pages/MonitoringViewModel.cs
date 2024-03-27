using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Models;
using Venv.Models.DockerHandler;

namespace Venv.ViewModels.Pages
{
    public partial class MonitoringViewModel : ObservableObject
    {
        private readonly VmMonitoringService _monitoringService;

        [ObservableProperty]
        private double _cpuUsage;
        [ObservableProperty]
        private string _cpuUsageText;
        [ObservableProperty]
        private double fillWidth;
        [ObservableProperty]
        private double progressBarWidth;


        partial void OnProgressBarWidthChanged(double value)
        {
            FillWidth = value * (CpuUsage / 100.0);
        }

        partial void OnCpuUsageChanged(double value)
        {
            CpuUsageText = $"{value}%";
            FillWidth = progressBarWidth * (value / 100.0);
        }
        [ObservableProperty]
        private int _activeCores = 8;
        [ObservableProperty]
        private int _totalCores = 24; //static for testing

        [ObservableProperty]
        private string _coreUsageText =$"8/24";
        [ObservableProperty]
        private double _memoryUsagePercentage;
        [ObservableProperty]
        private string _memoryUsageText;

        public MonitoringViewModel(VMwareManager vmWareManager) 
        {
             _monitoringService = new VmMonitoringService(vmWareManager.IP);
            _ = LoadPerformanceDataAsync();
        }

         
        public async Task LoadPerformanceDataAsync()
        {
            while (true)
            {
                var data = await _monitoringService.GetPerformanceDataAsync();
                CpuUsage = data.CpuUsage;
                MemoryUsagePercentage = data.memoryPercent;
                MemoryUsageText = $"{data.usedMemory}/{data.freeMemory}";
                await Task.Delay(1000);
            }      
        }
    }
}
