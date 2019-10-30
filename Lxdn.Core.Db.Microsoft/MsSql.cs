
using System.Data.SqlClient;

namespace Lxdn.Core.Db.Microsoft
{
    public class MsSql : Database<SqlConnection, MsSqlBehaviors>
    {
        public MsSql(Schema schema) : base(schema) { }

        public static IDatabase Use(Schema schema) => new MsSql(schema);
    }

    public class MsSqlBehaviors : DbBehaviors<SqlCommand, SqlParameter> { }
}