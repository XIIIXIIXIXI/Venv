using Microsoft.UI.Xaml.Controls;

namespace Venv.Services
{
    public interface INavigationService
    {
        void NavigateTo<TPage>() where TPage : Page;

        void SetFrame(Frame frame);
    }
}
