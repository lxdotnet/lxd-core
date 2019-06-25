using System;
using System.Collections.Generic;
using Lxd.Core.Extensions;

namespace Lxd.Core.Db
{
    internal static class BufferSize
    {
        private static readonly Dictionary<Type, int> BufferSizeOf = new Dictionary<Type, int>
        {
            [typeof(string)] = 256,
            [typeof(Guid)] = 32
        };

        public static int? Of(Type type)
        {
            return BufferSizeOf.ContainsKey(type).IfExists(contains => BufferSizeOf[type]);
        }
    }
}