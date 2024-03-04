using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Venv.ViewModels.Pages;
using Venv.Views.Pages;

namespace Venv.Services
{
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }
        public bool NavigateTo(Type viewModelType, object parameter = null)
        {
            
            var viewName = viewModelType.Name.Replace("ViewModel", "Page");
            var viewType = Type.GetType("Venv.Views.Pages." + viewName);
            if (viewType == null)
                return false;

            bool navigated = _frame.Navigate(viewType, parameter);

            if (navigated)
            {
                var page = (Page)_frame.Content;
                var viewModel = _serviceProvider.GetRequiredService(viewModelType);
                page.DataContext = viewModel;
            }

            return navigated;
        }
        
        public void GoBack()
        {
            if (_frame?.CanGoBack == true)
            {
                _frame.GoBack();
            }
        }
    }
}
