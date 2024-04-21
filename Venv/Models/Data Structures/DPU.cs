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

        //This a hack that should be changed in the future. ShipDataService is updating the status, but the view can only reflect it
        // if it is updated on the main thread using dispatcher que. ShipDataService don't want to use dispatcherQue since it 
        //doesen't align with the MVVM-pattern. ShipDataService is now updating StatusHolder while the viewModel take care of the ObservaleProperty _status.
        public string StatusHolder { get; set; }
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
