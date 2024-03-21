using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Models
{
    public partial class DPU : ObservableObject
    {
        [ObservableProperty]
        private int _number;

        [ObservableProperty]
        private string _status = "Off";

        [ObservableProperty]
        private bool _isSelected = true;

        [ObservableProperty]
        private string _sioModuleID;


        public ObservableCollection<LineSetup> LineSetups { get; set; } = new ObservableCollection<LineSetup>();

    }

    public partial class LineSetup : ObservableObject
    {

        [ObservableProperty]
        private int _sioChannelNumber;
        [ObservableProperty]
        private string _channelInfo;

        [ObservableProperty]
        private int _baudRate;

        [ObservableProperty]
        private string _lineFormat;

        [ObservableProperty]
        private string _handshake;

        [ObservableProperty]
        private string _rsSpecification;
    }
}
