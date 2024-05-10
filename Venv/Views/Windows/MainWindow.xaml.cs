using Microsoft.UI.Xaml;
using System;
using System.Diagnostics.CodeAnalysis;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Venv
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            this.InitializeComponent();
        }
        
        public IntPtr GetWindowHandle()
        {
            return WindowNative.GetWindowHandle(this);
        }
    }
}

