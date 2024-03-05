using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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
    }
}
