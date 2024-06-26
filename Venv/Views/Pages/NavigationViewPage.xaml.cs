using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;
using Venv.Services;
using Venv.ViewModels.Pages;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class NavigationViewPage : Page
    {
        private readonly INavigationService _navigationService;
        public NavigationViewPage(INavigationService navigationService)
        {
            this.InitializeComponent();
            _navigationService = navigationService;
            NavigationView.ItemInvoked += OnNavigationViewItemInvoked;
        }

        private void OnNavigationViewItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var invokedItem = args.InvokedItemContainer as NavigationViewItem;
            if (invokedItem != null)
            {
                NavigateAndSetup(invokedItem.Tag.ToString());
            }
        }
        private void NavigateAndSetup(string pageTag)
        {
            _navigationService.SetFrame(ContentFrame);

            switch (pageTag)
            {
                case "Page1":
                    _navigationService.NavigateTo<SelectConfigurationPage>();
                    break;
                case "Page2":
                    _navigationService.NavigateTo<VirtualPage>();
                    break;
                case "Page3":
                    _navigationService.NavigateTo<DpuSelectionPage>();
                    break;
                case "Page4":
                    _navigationService.NavigateTo<MonitoringPage>();
                    break;
                default:
                    break;

            }

            
            if (ContentFrame.Content is SelectConfigurationPage configPage)
            {
                var viewModel = configPage.DataContext as SelectConfigurationViewModel;
                viewModel.ConfigurationSelectionChanged += UpdateNavigationItemsEnabledState;
            }
            else if (ContentFrame.Content is  VirtualPage virtualPage)
            {
                var viewModel = virtualPage.DataContext as VirtualViewModel;
                viewModel.VirtualizationRunningChanged += UpdateNavigationItemsEnabledStateEmulationRunning;
            }
        }
        private void UpdateNavigationItemsEnabledState(bool isConfigSelected)
        {
            foreach (NavigationViewItemBase item in NavigationView.MenuItems)
            {
                if (item is NavigationViewItem nvi)
                {
                    if (nvi.Tag.ToString() == "Page2" || nvi.Tag.ToString() == "Page3" || nvi.Tag.ToString() == "Page4")
                    {
                        nvi.IsEnabled = isConfigSelected;
                    }
                }
            }
        }
        private void UpdateNavigationItemsEnabledStateEmulationRunning(bool isEmulationRunning)
        {
            foreach (NavigationViewItemBase item in NavigationView.MenuItems)
            {
                if (item is NavigationViewItem nvi)
                {
                    if (nvi.Tag.ToString() == "Page1" || nvi.Tag.ToString() == "Page3")
                    {
                        nvi.IsEnabled = !isEmulationRunning;
                    }
                }
            }
        }
        private void NavigationViewPage_Loaded(object sender, RoutedEventArgs e)
        {
            //The viewitem doesent highlight when navigating automatically, this fixes it.
            foreach (NavigationViewItemBase item in NavigationView.MenuItems)
            {
                if (item is NavigationViewItem && (string)item.Tag == "Page1")
                {
                    NavigationView.SelectedItem = item;
                    break; 
                }
            }

            NavigateAndSetup("Page1");
        }

    }
}
