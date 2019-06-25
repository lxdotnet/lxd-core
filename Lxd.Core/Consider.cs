using System;
using System.Linq;

namespace Lxd.Core
{
    internal class Consider
    {
        private static readonly string[] stopList = { "mscorlib", "System", "Newtonsoft.Json" };

        public static bool ForIteration(Type candidate)
        {
            return !candidate.IsValueType && !stopList.Any(assembly => 
                string.Equals(assembly, candidate.Assembly.GetName().Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
