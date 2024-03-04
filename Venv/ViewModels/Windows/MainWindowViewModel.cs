using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Resources;
using Venv.Services;
using Venv.Views.Pages;
using Venv.ViewModels.Pages;

namespace Venv.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly INavigationService _navigationService;
        public MainWindowViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        [RelayCommand]
        private void NavigateButton()
        {
            _navigationService.NavigateTo(typeof(VirtualViewModel));
        }
  
    }
}
