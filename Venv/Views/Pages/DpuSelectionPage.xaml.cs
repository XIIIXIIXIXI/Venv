using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Venv.ViewModels.Pages;
using Venv.Models;
using System.Diagnostics.CodeAnalysis;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class DpuSelectionPage : Page
    {
        public DpuSelectionViewModel ViewModel { get; }
        public DpuSelectionPage(DpuSelectionViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            this.DataContext = ViewModel;
        }
        private void MachineryGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is GridView gridView)
            {
                var selectedGroups = gridView.SelectedItems.Cast<MachineryGroup>().ToList();
                ViewModel.UpdateDpuSelectionBasedOnGroups(selectedGroups);

            }
        }
    }
}
