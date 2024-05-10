using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Venv.Resources
{
    [ExcludeFromCodeCoverage]
    public static class ThemePaths
    {
        public static string DayColors => "ms-appx:///Resources/DayColors.xaml";
        public static string DuskColors => "ms-appx:///Resources/DuskColors.xaml";
        public static string NightColors => "ms-appx:///Resources/NightColors.xaml";
    }
    [ExcludeFromCodeCoverage]
    public static class VMPaths
    {
        //Work
        //public static string vmxPath => @"C:\Users\MKO091\OneDrive - Wärtsilä Corporation\Desktop\virtual_DPU_spawner\VM_virtualdpu_spawner\virtual_DPU_spawner.vmx";
        //home
        public static string vmxPath => @"C:\Users\marti\Desktop\VM\test.vmx";
        //for future:
        //public static string vmxPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "VM", "test.vmx");
        public static string vmrunPath => @"C:\Program Files (x86)\VMware\VMware VIX\vmrun";

        //Home
        public static string confTestPath => @"C:\IM_DBs\MaerskTank - 2.1.16.06\MaerskTank - 2.1.16.06";

        //Work
        //public static string confTestPath => @"C:\IM_DBs\MaerskTank - 2.1.16.06";
    }
}
