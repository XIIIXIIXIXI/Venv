using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics.CodeAnalysis;
using Venv.ViewModels.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed partial class MonitoringPage : Page
    {
        public MonitoringViewModel ViewModel { get; }
        public MonitoringPage(MonitoringViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            this.DataContext = ViewModel;
            
        }
        private void ProgressBarContainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.ProgressBarWidth = e.NewSize.Width;
        }
    }
}
