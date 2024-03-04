﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Venv.Services;
using Venv.ViewModels.Pages;
using Venv.ViewModels.Windows;
using Venv.Views.Pages;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            this.InitializeComponent();
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    //services, windows and view models
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<MainWindowViewModel>();
                    services.AddSingleton<INavigationService>((IServiceProvider serviceProvider) =>
                new NavigationService(serviceProvider));

                    services.AddSingleton<VirtualPage>();
                    services.AddSingleton<VirtualViewModel>();
                    //host services
                }).Build();
        }

        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            await _host.StartAsync();

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            var navigationService = _host.Services.GetRequiredService<INavigationService>();

            mainWindow.InitializeNavigationService(navigationService);

            mainWindow.Activate();
        }
    }
}
