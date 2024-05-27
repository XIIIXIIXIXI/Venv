using CommunityToolkit.Mvvm.ComponentModel;
using DevExpress.WinUI.Charts.Internal;
using DevExpress.WinUI.Charts;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Venv.Models.Services;
using Venv.Models.Interfaces;
using System.Diagnostics.CodeAnalysis;
using Venv.Models.Data_Access;
using Venv.Resources;
using CommunityToolkit.Mvvm.Input;
using DevExpress.ClipboardSource.SpreadsheetML;

namespace Venv.ViewModels.Pages
{
    public partial class MonitoringViewModel : ObservableObject
    {
        private readonly VmMonitoringService _monitoringService;

        [ObservableProperty]
        private double yAxisStartValue  = 0;
        [ObservableProperty]
        private double yAxisEndValue  = 100;

        [ObservableProperty]
        private double _cpuUsage;
        [ObservableProperty]
        private string _cpuUsageText;
        [ObservableProperty]
        private double fillCpuUsageWidth;
        [ObservableProperty]
        private double progressBarWidth;

        public CpuUsageDataSource CpuUsageChartDataSource { get; }

        [ObservableProperty]
        private ObservableCollection<CpuUsageDataItem> _cpuUsagePoints;


        partial void OnProgressBarWidthChanged(double value)
        {
            FillCpuUsageWidth = value * (CpuUsage / 100.0);
            FillMemoryUsageWidth = value * (MemoryUsagePercentage / 100);
        }

        partial void OnCpuUsageChanged(double value)
        {
            CpuUsageText = $"{value}%";
            FillCpuUsageWidth = progressBarWidth * (value / 100.0);
        }
        

        [ObservableProperty]
        private double _memoryUsagePercentage;
        [ObservableProperty]
        private string _memoryUsageText;
        [ObservableProperty]
        private double fillMemoryUsageWidth;
        partial void OnMemoryUsagePercentageChanged(double value)
        {
            MemoryUsageText = $"{value}%";
            FillMemoryUsageWidth = progressBarWidth * (value / 100.0);
        }


        private readonly IDispatcherQueue _dispatcherQueue;
        private VmxConfig _vmmxConfig;
        private IVMwareManager _vmwareManger;
       
        public MonitoringViewModel(IVMwareManager vmWareManager, IDispatcherQueue dispatcherQueue) 
        {
            _dispatcherQueue = dispatcherQueue;
            _vmwareManger = vmWareManager;
            _monitoringService = new VmMonitoringService(vmWareManager.IP);
            _cpuUsagePoints = new ObservableCollection<CpuUsageDataItem>();

            _vmmxConfig = new VmxConfig();
            (TotalProcessors, ActiveProcessor) = _vmmxConfig.ReadVmxConfiguration(VMPaths.vmxPath);
            SelectedProcessors = ActiveProcessor;
            UpdateProcessorOptions();

            CpuUsageChartDataSource = new CpuUsageDataSource(_cpuUsagePoints);
            _ = LoadPerformanceDataAsync();
        }

         
        public async Task LoadPerformanceDataAsync()
        {
            while (!IsRestarting)
            {
                var data = await _monitoringService.GetPerformanceDataAsync();
                CpuUsage = data.CpuUsage;
                MemoryUsagePercentage = data.memoryPercent;
                MemoryUsageText = $"{data.usedMemory}/{data.totalMemory}";
                var timestamp = DateTime.Now;

                _dispatcherQueue.TryEnqueue(() =>
                {
                    _cpuUsagePoints.Add(new CpuUsageDataItem
                    {
                        Timestamp = timestamp,
                        CpuUsage = CpuUsage
                    }) ;
                    

                    // limit the number of data points
                    if (_cpuUsagePoints.Count > 100)
                    {
                        _cpuUsagePoints.RemoveAt(0);
                    }
                });
                await Task.Delay(1000);
            }      
        }


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RestartButtonVisibility))]
        private int _activeProcessor = 8;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RestartButtonVisibility))]
        private int _selectedProcessors;
        [ObservableProperty]
        private int _totalProcessors;
        [ObservableProperty]
        private bool _isRestarting;

        public string RestartButtonVisibility => IsRestarting ? "Collapsed" : (SelectedProcessors != ActiveProcessor ? "Visible" : "Collapsed");
        public ObservableCollection<int> ProcessorOptions { get; } = new ObservableCollection<int>();

        public void UpdateProcessorOptions()
        {
            ProcessorOptions.Clear();

            int[] predefinedOptions = { 1, 2, 3, 4, 6, 8, 12, 16, 24, 32 };
            foreach (int option in predefinedOptions)
            {
                if (option <= _totalProcessors)
                {
                    ProcessorOptions.Add(option);
                }
                else
                {
                    break; //stop adding more option since the host limit is reached. 
                }
            }
        }
        [RelayCommand]
        public async void OnRestartButtonClick()
        {
            IsRestarting = true;

            await Task.Run(async () =>
            {
                await _monitoringService.StopMonitoringAsync();

                _dispatcherQueue.TryEnqueue(() => {
                    CpuUsage = 0;
                    MemoryUsagePercentage = 0;
                    MemoryUsageText = "0";
                    ActiveProcessor = 0;
                    var timestamp = DateTime.Now;
                    _cpuUsagePoints.Add(new CpuUsageDataItem
                    {
                        Timestamp = timestamp,
                        CpuUsage = 0
                    });
                });

                _vmwareManger.StopVMwareInstance();
                _vmmxConfig.UpdateVmxProcessors(VMPaths.vmxPath, SelectedProcessors);
                _vmwareManger.StartVMwareInstance();

                _monitoringService.StartMonitoring();

                
            });

            _dispatcherQueue.TryEnqueue(() => {
                (TotalProcessors, ActiveProcessor) = _vmmxConfig.ReadVmxConfiguration(VMPaths.vmxPath);
                IsRestarting = false;
                _ = LoadPerformanceDataAsync();
            });

        }

    }
    public struct CpuUsageDataItem
    {
        public DateTime Timestamp { get; set; }
        public double CpuUsage { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class CpuUsageDataSource : DataSourceBase
    {
        private ObservableCollection<CpuUsageDataItem> _data;

        public CpuUsageDataSource(ObservableCollection<CpuUsageDataItem> data)
        {
            _data = data;
            _data.CollectionChanged += (s, e) => OnDataChanged(ChartDataUpdateType.Reset, -1);
        }

        protected override int RowsCount => _data.Count;

        protected override object GetKey(int index) => _data[index].Timestamp;

        protected override double GetNumericalValue(int index, ChartDataMemberType dataMemberType)
        {
            if (dataMemberType == ChartDataMemberType.Value)
            {
                return _data[index].CpuUsage;
            }
            return 0;
        }

        protected override DateTime GetDateTimeValue(int index, ChartDataMemberType dataMemberType)
        {
            if (dataMemberType == ChartDataMemberType.Argument)
            {
                return _data[index].Timestamp;
            }
            return DateTime.MinValue;
        }

        protected override ActualScaleType GetScaleType(ChartDataMemberType dataMember)
        {
            switch (dataMember)
            {
                case ChartDataMemberType.Argument:
                    return ActualScaleType.DateTime; // X-axis is DateTime
                case ChartDataMemberType.Value:
                    return ActualScaleType.Numerical; // Y-axis is Numerical
                default:
                    throw new ArgumentOutOfRangeException(nameof(dataMember), "Unsupported data member type.");
            }
        }

        protected override string GetQualitativeValue(int index, ChartDataMemberType dataMember)
        {
            return string.Empty;
        }
    }

}
