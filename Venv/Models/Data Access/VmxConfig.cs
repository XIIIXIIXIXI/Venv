using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Venv.Models.Data_Access
{
    public class VmxConfig
    {
        public VmxConfig()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
        public (int maxForcedSampleCount, int numVcpus) ReadVmxConfiguration(string vmxFilePath)
        {
            int maxForcedSampleCount = 0;
            int numVcpus = 0;

            if (!File.Exists(vmxFilePath))
            {
                throw new FileNotFoundException("The specificed VMX file was not found");
            }

            foreach (var line in File.ReadAllLines(vmxFilePath))
            {
                if (line.StartsWith("vmotion.svga.maxForcedSampleCount"))
                {
                    var valuePart = line.Split('=')[1].Trim().Trim('"');
                    int.TryParse(valuePart, out maxForcedSampleCount);
                }
                else if (line.StartsWith("numvcpus"))
                {
                    var valuePart = line.Split('=')[1].Trim().Trim('"');
                    int.TryParse (valuePart, out numVcpus);
                }
            }
            return (maxForcedSampleCount, numVcpus);
        }

        public void UpdateVmxProcessors(string vmxFilePath, int selectedProcessors)
        {
            string tempFilePath = Path.GetTempFileName(); // Create a temporary file

            try
            {
                using (var input = File.OpenRead(vmxFilePath))
                using (var output = new StreamWriter(tempFilePath, false, Encoding.GetEncoding("Windows-1252")))
                {
                    using (var reader = new StreamReader(input, Encoding.GetEncoding("Windows-1252")))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            // Modify the lines that need to be updated
                            if (line.StartsWith("cpuid.coresPerSocket"))
                            {
                                line = $"cpuid.coresPerSocket = \"{selectedProcessors}\"";
                            }
                            else if (line.StartsWith("numvcpus"))
                            {
                                line = $"numvcpus = \"{selectedProcessors}\"";
                            }

                            output.WriteLine(line); // Write the original or modified line to the temporary file
                        }
                    }
                }

                // Replace the original file with the updated temporary file
                File.Delete(vmxFilePath);
                File.Move(tempFilePath, vmxFilePath);
            }
            catch
            {
                // If an error occurs, delete the temporary file
                File.Delete(tempFilePath);
                throw; // Re-throw the exception to handle it elsewhere
            }
        }
    }
}
