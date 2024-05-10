using System;
using System.Diagnostics.CodeAnalysis;

namespace Venv.Services
{

    public interface IWindowHandleProvider
    {
        IntPtr GetWindowHandle();
    }
    [ExcludeFromCodeCoverage]
    public class WindowHandleProvider : IWindowHandleProvider
    {
        private Func<IntPtr> _getWindowHandle;

        public WindowHandleProvider(Func<IntPtr> getWindowHandle)
        {
            _getWindowHandle = getWindowHandle;
        }

        public IntPtr GetWindowHandle() => _getWindowHandle();

        public void SetWindowHandle(Func<IntPtr> getWindowHandle)
        {
            _getWindowHandle = getWindowHandle;
        }
    }
}
