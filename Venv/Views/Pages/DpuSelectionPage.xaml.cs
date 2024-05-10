using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
        private void SelectAll_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectAllDPUs(true);
        }
        private void SelectAll_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectAllDPUs(false);
        }
    }
}
