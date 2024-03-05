using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Services
{
    public interface INavigationService
    {
        void NavigateTo<TPage>() where TPage : Page;
    }
}
