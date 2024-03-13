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
using Microsoft.UI.Xaml.Controls;
using Venv.Views.Pages;

namespace Venv.ViewModels.Pages
{
    public partial class SelectConfigurationViewModel : ObservableObject
    {
        private readonly IWindowHandleProvider _windowHandleProvider;
        private readonly INavigationService _navigationService;
        public SelectConfigurationViewModel(IWindowHandleProvider windowHandleProvider, INavigationService navigationService)
        {
            _navigationService = navigationService;
            SelectFolderCommand = new AsyncRelayCommand(SelectFolderAsync);
            NavigateToShell = new RelayCommand(NavigateToNewFrame);
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
        private bool isConfigurationSelected = false;
        [ObservableProperty] //only used to update the listview when selecting from file explorer
        private ConfigurationModel selectedConfiguration;
        [ObservableProperty]
        private ObservableCollection<ConfigurationModel> recentConfigurations = new();

        public IAsyncRelayCommand SelectFolderCommand { get; }
        public IRelayCommand NavigateToShell { get; }

        private void NavigateToNewFrame()
        {
            _navigationService.NavigateTo<NavigationViewPage>();
        }
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
                SelectedConfiguration = RecentConfigurations.FirstOrDefault(c => c.FilePath.Equals(folder.Path, StringComparison.OrdinalIgnoreCase));
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
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy");
            var configDetails = $"{vesselName}|{filePath}|{currentDate}";

            var existingConfigIndex = RecentConfigurations.IndexOf(RecentConfigurations.FirstOrDefault(c => c.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)));
            if (existingConfigIndex != -1)
            {
                RecentConfigurations.RemoveAt(existingConfigIndex);
            }

            RecentConfigurations.Insert(0, new ConfigurationModel
            {
                VesselName = vesselName,
                FilePath = filePath,
                LastUsed = DateTime.ParseExact(currentDate, "dd/MM/yyyy", null)
            });

            var updatedFileContent = RecentConfigurations.Select(c => $"{c.VesselName}|{c.FilePath}|{c.LastUsed:dd/MM/yyyy}").ToList();
            var file = Path.Combine(ApplicationData.Current.LocalFolder.Path, "RecentConfigurations.txt");

            await File.WriteAllLinesAsync(file, updatedFileContent);

            
        }

        private async Task LoadRecentConfigurationsAsync()
        {
            var file = Path.Combine(ApplicationData.Current.LocalFolder.Path, "RecentConfigurations.txt");

            if (File.Exists(file))
            {
                var lines = await File.ReadAllLinesAsync(file);
                foreach (var line in lines.Reverse())
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        RecentConfigurations.Add(new ConfigurationModel
                        {
                            VesselName = parts[0],
                            FilePath = parts[1],
                            LastUsed = DateTime.ParseExact(parts[2], "dd/MM/yyyy", null)
                        });
                    }
                }
            }
        }

        private async void ShowInfoBar(string message)
        {
            InfoBarMessage = message;
            IsInfoBarOpen = true;

            await Task.Delay(30000);
            IsInfoBarOpen = false;
        }

    }
}
