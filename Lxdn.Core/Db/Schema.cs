using System;
using System.Configuration;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Db
{
    public class Schema
    {
        #if NETFULL

        //public Schema(ConnectionStringSettings settings) : this(settings.Name, settings.ConnectionString) { }
        public static Schema CreateFrom(ConnectionStringSettings settings) 
            => new Schema { ConnectionString = settings.ConnectionString };

        #endif

        public string ConnectionString { get; set; }
    }
}
