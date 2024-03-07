using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.Services;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace Venv.ViewModels.Pages
{
    public partial class SelectConfigurationViewModel : ObservableObject
    {
        [ObservableProperty]
        private ShipDataService _shipDataService;
        private readonly IWindowHandleProvider _windowHandleProvider;

        public SelectConfigurationViewModel(IWindowHandleProvider windowHandleProvider)
        {
            SelectFolderCommand = new AsyncRelayCommand(SelectFolderAsync);
            _windowHandleProvider = windowHandleProvider;
        }
        public IAsyncRelayCommand SelectFolderCommand { get; }

        private async Task SelectFolderAsync()
        {
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            IntPtr hwnd = _windowHandleProvider.GetWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            

        }
    }
}
