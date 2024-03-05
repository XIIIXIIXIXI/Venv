using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using Venv.Models;


namespace Venv.ViewModels.Pages
{
    public partial class VirtualViewModel : ObservableObject
    {
        public ObservableCollection<DPU> Dpus { get; } = new();
        public VirtualViewModel()
        {
        }
    }
}

