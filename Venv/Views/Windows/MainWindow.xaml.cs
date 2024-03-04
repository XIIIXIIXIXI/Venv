using CommunityToolkit.Mvvm.ComponentModel;
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
using Venv.Resources;
using Venv.Services;
using Venv.ViewModels.Windows;
using Venv.Views.Pages;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainWindow : Window 
    {
        public MainWindowViewModel ViewModel { get; }
        public MainWindow(
            MainWindowViewModel viewModel
        )
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            RootPanel.DataContext = ViewModel;
        }
        public void InitializeNavigationService(INavigationService navigationService)
        {
            navigationService.Initialize(MainNavigationFrame);
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries.Clear();
            var newThemeUri = new Uri(ThemePaths.DayColors);
            var newTheme = new ResourceDictionary { Source = newThemeUri };
            Application.Current.Resources.MergedDictionaries.Add(newTheme);

            if (Application.Current.Resources.TryGetValue("WindBack", out var newBackground))
            {
                RootPanel.Background = (Brush)newBackground; 
            }
        }

    }
}
