using System;

namespace Lxd.Core.Injection
{
    /// <summary>
    /// When an object is IEnumerable, do not enumerate it (e.g. when injecting)
    /// </summary>
    public class TreatAsObjectAttribute : Attribute { }
}