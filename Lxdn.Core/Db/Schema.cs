using System;
using System.Configuration;
using Lxd.Core.Extensions;

namespace Lxd.Core.Db
{
    public class Schema
    {
        public Schema(string name, string connectionString)
        {
            Name = name.ThrowIf(string.IsNullOrEmpty, x => new ArgumentNullException(nameof(name)));
            ConnectionString = connectionString.ThrowIf(string.IsNullOrEmpty, x => new ArgumentNullException(nameof(connectionString)));
        }

        #if NETFULL

        public Schema(ConnectionStringSettings settings) : this(settings.Name, settings.ConnectionString) { }

        #endif

        public string Name { get; }

        public string ConnectionString { get;  }
    }
}
