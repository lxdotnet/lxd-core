using System;
using System.Data;
using System.Linq;
using Lxd.Core.Extensions;
using System.Collections.Generic;

namespace Lxd.Core.Db
{
    internal static class DbTypeMapper
    {
        private static readonly Dictionary<Type, DbType> DbTypeOf = new Dictionary<Type, DbType>
        {
            // not complete, just copy-paste from my old legacy
            [typeof(string)] = DbType.String,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(long)] = DbType.Int64,
            [typeof(int)] = DbType.Int32,
            [typeof(short)] = DbType.Int16,
            [typeof(byte)] = DbType.Byte,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(byte[])] = DbType.Object, // byte[] / blob
        };

        public static DbType? ToDbType(this Type type)
        {
            var nullableOf = type.AsArgumentsOf(typeof(Nullable<>));

            return nullableOf.HasValue 
                ? nullableOf.Value.Single().ToDbType() 
                : DbTypeOf.ContainsKey(type).IfExists(contains => DbTypeOf[type]);
        }
    }
}