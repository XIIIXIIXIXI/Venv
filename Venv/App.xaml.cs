using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics.CodeAnalysis;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.Services;
using Venv.ViewModels.Pages;
using Venv.Views.Pages;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class App : Application
    {
        private IServiceProvider _serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {


            services.AddSingleton<MainWindow>();
            services.AddSingleton<Frame>(sp =>
            {
                var mainWindow = sp.GetRequiredService<MainWindow>();
                var frame = new Frame();
                mainWindow.Content = frame;
                return frame;
            });
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<VirtualPage>();
            services.AddSingleton<VirtualViewModel>();
            services.AddSingleton<SelectConfigurationPage>();
            services.AddSingleton<DpuSelectionPage>();
            services.AddSingleton<DpuSelectionViewModel>();
            services.AddTransient<NavigationViewPage>();
            services.AddSingleton<MonitoringPage>();
            services.AddSingleton<MonitoringViewModel>();
            services.AddSingleton<Mediator>();

            services.AddSingleton<IVMwareManager, VMwareManager>();
            services.AddSingleton<SelectConfigurationViewModel>();
            services.AddSingleton<IShipDataService, ShipDataService>();
            services.AddSingleton<IDispatcherQueue, DispatcherQueueWrapper>();



            services.AddSingleton<IWindowHandleProvider, WindowHandleProvider>((sp) =>
            new WindowHandleProvider(() => throw new InvalidOperationException("Window handle not initialized yet.")));
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            //activate main window
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

            mainWindow.Activate();

            // Retrieve the window handle from the activated MainWindow (pointer)
            IntPtr mainWindowHandle = mainWindow.GetWindowHandle();


            var windowHandleProvider = (WindowHandleProvider)_serviceProvider.GetRequiredService<IWindowHandleProvider>();
            windowHandleProvider.SetWindowHandle(() => mainWindowHandle);

            var navigationService = _serviceProvider.GetRequiredService<INavigationService>();
            navigationService.NavigateTo<NavigationViewPage>();

            //ChangeWindowSize(mainWindowHandle, 1450, 850);
            //SetFullScreen(mainWindowHandle);
            SetFullScreenWindowed(mainWindowHandle);
        }

        private void ChangeWindowSize(IntPtr windowHandle, int width, int height)
        {
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            var newSize = new Windows.Graphics.SizeInt32 { Width = width, Height = height };
            appWindow.Resize(newSize);

            //Center
            var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
            var screenBounds = displayArea.WorkArea;

            var xPosition = screenBounds.X + ((screenBounds.Width - width) / 2);
            var yPosition = screenBounds.Y + ((screenBounds.Height - height) / 2);

            var newPosition = new Windows.Graphics.PointInt32(xPosition, yPosition);
            appWindow.Move(newPosition);
        }
        private void SetFullScreen(IntPtr windowHandle)
        {
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Set full screen
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
        private void SetFullScreenWindowed(IntPtr windowHandle)
        {
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.SetPresenter(AppWindowPresenterKind.Default);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 0, Height = 0 });
            appWindow.SetPresenter(AppWindowPresenterKind.Default);

            var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
            var screenBounds = displayArea.WorkArea;
            var newSize = new Windows.Graphics.SizeInt32 { Width = screenBounds.Width, Height = screenBounds.Height };

            appWindow.Resize(newSize);
            appWindow.Move(new Windows.Graphics.PointInt32(screenBounds.X, screenBounds.Y));

            //remove the old ugly looking window bar in the top.
            if (appWindow.TitleBar != null)
            {
                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(00,00,00,00);
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(00, 00, 00, 00);
            }
        }
    }
}
