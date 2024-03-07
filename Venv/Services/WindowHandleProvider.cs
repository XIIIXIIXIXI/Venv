using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Services
{
    public interface IWindowHandleProvider
    {
        IntPtr GetWindowHandle();
    }

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
