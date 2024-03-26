using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Venv.Services;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            _navigationService.SetFrame(ContentFrame);
            var invokedItem = args.InvokedItemContainer as NavigationViewItem;

            switch (invokedItem.Tag.ToString())
            {
                case "Page1":
                    _navigationService.NavigateTo<VirtualPage>();
                    break;
                case "Page2":
                    _navigationService.NavigateTo<DpuSelectionPage>();
                    break;
                case "Page3":
                    _navigationService.NavigateTo<MonitoringPage>();
                    break;
                default:
                    break;
            }
        }

    }
}
