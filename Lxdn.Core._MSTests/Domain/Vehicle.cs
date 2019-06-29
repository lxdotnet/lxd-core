using System.Collections.Generic;

namespace Lxdn.Core._MSTests.Domain
{
    internal class Vehicle
    {
        public string Manufacturer { get; set; }
        public List<string> EquipmentItems { get; set; } // List to be able to inject simple types like string -> to be fixed
    }
}
