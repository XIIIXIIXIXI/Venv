using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venvs.Resources
{
    static class ThemePaths
    {
        public static string DayColors => "ms-appx:///Resources/DayColors.xaml";
        public static string DuskColors => "ms-appx:///Resources/DuskColors.xaml";
        public static string NightColors => "ms-appx:///Resources/NightColors.xaml";
    }

    static class VMPaths
    {
        public static string vmxPath => @"C:\Users\MKO091\OneDrive - Wärtsilä Corporation\Desktop\virtual_DPU_spawner\VM_virtualdpu_spawner\virtual_DPU_spawner.vmx";
        public static string vmrunPath => @"C:\Program Files (x86)\VMware\VMware VIX\vmrun";
    }
}
