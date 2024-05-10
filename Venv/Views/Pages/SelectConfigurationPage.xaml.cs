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
    public sealed partial class SelectConfigurationPage : Page
    {
        SelectConfigurationViewModel ViewModel { get; }
        public SelectConfigurationPage(SelectConfigurationViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
            this.DataContext = ViewModel;         
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ConfigurationModel clickedItem)
            {
                ViewModel.LoadConfigurationCommand.Execute(clickedItem);
            }
        }
        private void Navigate_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.NavigateAwayFromFrame();
        }
    }
}
