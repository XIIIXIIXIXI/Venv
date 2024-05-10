using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Linq;
using System.Threading.Tasks;
using Venv.Services;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using Venv.Models;
using System.Globalization;
using Venv.Models.Services;
using Venv.Models.Interfaces;


namespace Venv.ViewModels.Pages
{
    
    public partial class SelectConfigurationViewModel : ObservableObject
    {
        private readonly IWindowHandleProvider _windowHandleProvider;
        private readonly IShipDataService _shipDataService;
        private readonly IVMwareManager _vmwareManager;
        private string _folderPath;
        public SelectConfigurationViewModel(IWindowHandleProvider windowHandleProvider, IShipDataService shipDataService, IVMwareManager vMwareManager)
        {
            _shipDataService = shipDataService;
            _vmwareManager = vMwareManager;
            _ = StartVMasync();
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

        public event Action<bool> ConfigurationSelectionChanged;
        [ObservableProperty]
        private bool isConfigurationSelected = false;
        partial void OnIsConfigurationSelectedChanged(bool value)
        {
            ConfigurationSelectionChanged?.Invoke(value);
        }

        [ObservableProperty] //only used to update the listview when selecting from file explorer
        private ConfigurationModel selectedConfiguration;
        [ObservableProperty]
        private ObservableCollection<ConfigurationModel> recentConfigurations = new();

        public IAsyncRelayCommand SelectFolderCommand { get; }
        

        public void NavigateAwayFromFrame()
        {
            _ = SaveConfigurationAsync(ShipData.VesselName, _folderPath);
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
                    _ = ShowInfoBar("The selected folder is incorrect. Please choose a valid configuration");
                    return;
                }
                ShipData = service;
                IsConfigurationSelected = true;
                _folderPath = folder.Path;
                await SaveConfigurationAsync(service.VesselName, folder.Path);
                SelectedConfiguration = RecentConfigurations.FirstOrDefault(c => c.FilePath.Equals(folder.Path, StringComparison.OrdinalIgnoreCase));
                _shipDataService.UpdateShipData(ShipData.DatabaseVersion, ShipData.DPUVersion, ShipData.NumberOfMFD, ShipData.VesselName, ShipData.IMO, ShipData.GetDpus(), ShipData.MachineryGroup, ShipData.YardBuildNumber, ShipData.SequenceNumber, ShipData.Yard, ShipData.FicVersion, ShipData.SwitchesNumber, ShipData.ShipOwner, ShipData.ShipType, ShipData.GenerationDate);
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
                    _folderPath = configuration.FilePath;
                    ShipData = service;
                    IsConfigurationSelected = true;
                    //updating the ship singleton
                    _shipDataService.UpdateShipData(ShipData.DatabaseVersion, ShipData.DPUVersion, ShipData.NumberOfMFD, ShipData.VesselName, ShipData.IMO, ShipData.GetDpus(), ShipData.MachineryGroup, ShipData.YardBuildNumber, ShipData.SequenceNumber, ShipData.Yard, ShipData.FicVersion, ShipData.SwitchesNumber, ShipData.ShipOwner, ShipData.ShipType, ShipData.GenerationDate);
                }
            }
        }

        private async Task SaveConfigurationAsync(string vesselName, string filePath)
        {
            var currentDate = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var existingConfigIndex = RecentConfigurations.IndexOf(RecentConfigurations.FirstOrDefault(c => c.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase)));
            if (existingConfigIndex != -1)
            {
                RecentConfigurations.RemoveAt(existingConfigIndex);
            }

            RecentConfigurations.Insert(0, new ConfigurationModel
            {
                VesselName = vesselName,
                FilePath = filePath,
                LastUsed = DateTime.ParseExact(currentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)
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
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 3)
                    {
                        RecentConfigurations.Add(new ConfigurationModel
                        {
                            VesselName = parts[0],
                            FilePath = parts[1],
                            LastUsed = DateTime.ParseExact(parts[2], "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        });
                    }
                }
            }
        }
        private async Task StartVMasync()
        {
            await Task.Run(() =>
            {
                _vmwareManager.StartVMwareInstance();
            });
        }

        private async Task ShowInfoBar(string message)
        {
            InfoBarMessage = message;
            IsInfoBarOpen = true;

            await Task.Delay(8000);
            IsInfoBarOpen = false;
        }

    }
}
