#if NETFULL

// MSSQL out of the box is currently available for classical .net framework only
// to add the support for .net core see https://stackoverflow.com/questions/35444487/how-to-use-sqlclient-in-asp-net-core

using System.Data.SqlClient;

namespace Lxd.Core.Db.SqlServer
{
    public class MsSql : Database<SqlConnection, MsSqlBehaviors>
    {
        public MsSql(Schema schema) : base(schema) { }

        public static IDatabase Use(Schema schema) => new MsSql(schema);
    }
}

#endif