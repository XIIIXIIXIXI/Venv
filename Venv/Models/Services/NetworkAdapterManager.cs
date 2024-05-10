using System;
using System.Linq;
using System.Management;

namespace Venv.Models.Services
{
    public class NetworkAdapterManager
    {
        private const string AdapterName = "VMware Virtual Ethernet Adapter for VMnet1";
        private const string IP = "172.16.1.4";
        private const string SubnetMask = "255.255.0.0";

        public bool CheckConfiguration()
        {
            using (var searcher = new ManagementObjectSearcher(
                "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = True"))
            {
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    var name = (string)queryObj["Description"];
                    if (name == AdapterName)
                    {
                        var ipAddresses = (string[])queryObj["IPAddress"];
                        var subnetMasks = (string[])queryObj["IPSubnet"];

                        if (ipAddresses != null && ipAddresses.Contains(IP) &&
                            subnetMasks != null && subnetMasks.Contains(SubnetMask))
                        {
                            return true; // Correct configuration
                        }
                        else
                        {
                            return false; // Incorrect configuration
                        }
                    }
                }
            }
            return false; // Adapter not found
        }
        public void Configure()
        {
            using (var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            using (var networkAdapters = managementClass.GetInstances())
            {
                foreach (ManagementObject adapter in networkAdapters)
                {
                    var name = (string)adapter["Description"];
                    if (name == AdapterName && (bool)adapter["IPEnabled"])
                    {
                        try
                        {
                            using (var newIP = adapter.GetMethodParameters("EnableStatic"))
                            {
                                newIP["IPAddress"] = new string[] { IP };
                                newIP["SubnetMask"] = new string[] { SubnetMask };

                                adapter.InvokeMethod("EnableStatic", newIP, null);
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                    }
                }
            }

        }
    }

}
