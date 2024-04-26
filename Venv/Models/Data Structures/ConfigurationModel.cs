using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
