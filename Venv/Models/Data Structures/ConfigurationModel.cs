using System;
using System.Diagnostics.CodeAnalysis;


namespace Venv.Models
{
    [ExcludeFromCodeCoverage]
    public class ConfigurationModel
    {
        public string VesselName { get; set; }
        public string FilePath { get; set; }

        public DateTime LastUsed {  get; set; }
    }
}
