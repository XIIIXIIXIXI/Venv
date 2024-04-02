using CommunityToolkit.Mvvm.ComponentModel;
using DevExpress.WinUI.Charts.Internal;
using DevExpress.WinUI.Charts;
using DevExpress.WinUI.Grid;
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
using System.Threading;
using Microsoft.UI.Dispatching;
using Windows.System;

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
        private int _activeCores = 8;
        [ObservableProperty]
        private int _totalCores = 24; //static for testing

        [ObservableProperty]
        private string _coreUsageText =$"8/24"; //TODO
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


        private readonly Microsoft.UI.Dispatching.DispatcherQueue _dispatcherQueue;

        public MonitoringViewModel(VMwareManager vmWareManager) 
        {
            _dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            _monitoringService = new VmMonitoringService(vmWareManager.IP);
            _cpuUsagePoints = new ObservableCollection<CpuUsageDataItem>();

            CpuUsageChartDataSource = new CpuUsageDataSource(_cpuUsagePoints);
            _ = LoadPerformanceDataAsync();
        }

         
        public async Task LoadPerformanceDataAsync()
        {
            while (true)
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

    }
    public struct CpuUsageDataItem
    {
        public DateTime Timestamp { get; set; }
        public double CpuUsage { get; set; }
    }
   
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
