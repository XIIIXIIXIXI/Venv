using System.Collections.Generic;


namespace Venv.Models
{
    public class MachineryGroup
    {
        public int Number { get; set; }
        public string Name { get; set; }

        public List<DPU> DPUs { get; set; } = new List<DPU>();
    }
}
