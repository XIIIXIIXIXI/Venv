using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
using System.Threading;
using Microsoft.Extensions.Primitives;
using System.IO;
using System.Collections.ObjectModel;
using Venv.Models;

namespace Venv.ViewModels.Pages
{
    public partial class SelectConfigurationViewModel : ObservableObject
    {
        private readonly IWindowHandleProvider _windowHandleProvider;
        public SelectConfigurationViewModel(IWindowHandleProvider windowHandleProvider)
        {
            SelectFolderCommand = new AsyncRelayCommand(SelectFolderAsync);
            _windowHandleProvider = windowHandleProvider;
            Task.Run(LoadRecentConfigurationsAsync);
        }


        [ObservableProperty]
        private ShipDataService _shipData;
        [ObservableProperty]
        private string infoBarMessage;
        [ObservableProperty]
        private bool isInfoBarOpen;
        [ObservableProperty]
        private bool isConfigurationSelected;
        [ObservableProperty]
        private ObservableCollection<ConfigurationModel> recentConfigurations = new();




        public IAsyncRelayCommand SelectFolderCommand { get; }
        public IAsyncRelayCommand SelectedConfiguration {  get; }

        private async Task SelectFolderAsync()
        {
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add("*");
            IntPtr hwnd = _windowHandleProvider.GetWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            var folder = await picker.PickSingleFolderAsync();
            if (folder != null)
            {
                var factory = new ShipConfigurationFactory(folder.Path);
                var service = factory.Create();
                if (service == null)
                {
                    ShowInfoBar("The selected folder is incorrect. Please choose a valid configuration");
                    return;
                }
                ShipData = service;
                IsConfigurationSelected = true;
                await SaveConfigurationAsync(service.VesselName, folder.Path);
            }
        }

        [RelayCommand]
        public void LoadConfiguration(ConfigurationModel configuration)
        {
            if (configuration != null)
            {
                var factory = new ShipConfigurationFactory(configuration.FilePath);
                var service = factory.Create();
                if (service != null)
                {
                    ShipData = service;
                    IsConfigurationSelected = true;
                }
            }
        }

        private async Task SaveConfigurationAsync(string vesselName, string filePath)
        {
            if (!RecentConfigurations.Any(c => c.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)))
            {
                var configDetails = $"{vesselName}|{filePath}\n";
                var file = Path.Combine(ApplicationData.Current.LocalFolder.Path, "RecentConfigurations.txt");

                await File.AppendAllTextAsync(file, configDetails);
                RecentConfigurations.Add(new ConfigurationModel
                {
                    VesselName = vesselName,
                    FilePath = filePath
                });
            }
            
        }

        private async Task LoadRecentConfigurationsAsync()
        {
            var file = Path.Combine(ApplicationData.Current.LocalFolder.Path, "RecentConfigurations.txt");

            if (File.Exists(file))
            {
                var lines = await File.ReadAllLinesAsync(file);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 2)
                    {
                        RecentConfigurations.Add(new ConfigurationModel
                        {
                            VesselName = parts[0],
                            FilePath = parts[1]
                        });
                    }
                }
            }
        }

        private async void ShowInfoBar(string message)
        {
            InfoBarMessage = message;
            IsInfoBarOpen = true;

            await Task.Delay(3000);
            IsInfoBarOpen = false;
        }

    }
}
