using System;
using System.Linq;

namespace Lxd.Core.Iteration
{
    internal class Consider // used in the injection logic and in the conversion to dynamic
    {
        private static readonly string[] stopList = { "mscorlib", "System", "Newtonsoft.Json" };

        public static bool ForIteration(Type candidate)
        {
            return !candidate.IsValueType && !stopList.Any(assembly => 
                string.Equals(assembly, candidate.Assembly.GetName().Name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
