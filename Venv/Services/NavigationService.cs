using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Venv.Views.Pages;


namespace Venv.Services
{
    [ExcludeFromCodeCoverage]
    public class NavigationService : INavigationService
    {
        private Frame _frame;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(Frame frame, IServiceProvider serviceProvider)
        {
            _frame = frame;
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TPage>() where TPage : Page
        {
            var pageType = typeof(TPage);
            var page = _serviceProvider.GetService(pageType) as Page;
            _frame.Content = page;
        }
        
        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

    }
}
