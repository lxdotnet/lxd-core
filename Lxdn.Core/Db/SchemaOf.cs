using System;
using System.Reflection;
using Lxd.Core.Extensions;

namespace Lxd.Core.Db
{
    public class SchemaOf // to be used in concrete business
    {
        public Schema SomeSchema { get; set; }

        public static SchemaOf Productive = new SchemaOf
        {
            // add initialization of the schema(s) here
        };

        public static SchemaOf Staging = new SchemaOf
        {
            // add initialization of the schema(s) here
        };

        public static SchemaOf Test = new SchemaOf
        {
            // add initialization of the schema(s) here
        };

        public static SchemaOf Environment(Environment environment)
        {
            return (SchemaOf) typeof(SchemaOf)
                .GetField(environment.ToString(), BindingFlags.Static | BindingFlags.Public)
                .ThrowIfDefault(() => new ArgumentOutOfRangeException(nameof(environment)))
                .GetValue(null);
        }
    }
}