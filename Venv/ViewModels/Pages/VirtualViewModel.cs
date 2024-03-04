using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.ViewModels.Pages
{
    public partial class VirtualViewModel : ObservableObject
    {
        [ObservableProperty]
        private int count;

        public VirtualViewModel()
        {
            IncrementCommand = new RelayCommand(IncrementCount);
        }

        public IRelayCommand IncrementCommand { get; }

        private void IncrementCount()
        {
            Count++; 
        }
    }
}
