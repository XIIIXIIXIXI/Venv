using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Syncfusion.Licensing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Venv.Models.Interfaces;
using Venv.Models.Services;
using Venv.Services;
using Venv.ViewModels.Pages;
using Venv.Views.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input.Inking;
using Windows.UI.WindowManagement;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using LaunchActivatedEventArgs = Microsoft.UI.Xaml.LaunchActivatedEventArgs;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
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

            services.AddSingleton<VMwareManager>();

            //services.AddSingleton<ShipConfigurationFactory>(sp => { return new ShipConfigurationFactory(""); });
            services.AddSingleton<SelectConfigurationViewModel>();
            services.AddSingleton<ShipDataService>();
            services.AddSingleton<IDispatcherQueue, DispatcherQueueWrapper>();


            // Register WindowHandleProvider as a singleton in the DI container. Throws exception if invoked before being initialized in the OnLaunched methode.
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
