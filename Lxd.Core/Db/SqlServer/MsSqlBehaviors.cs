#if NETFULL

using System.Data.SqlClient;

namespace Lxd.Core.Db.SqlServer
{
    public class MsSqlBehaviors : DbBehaviors<SqlCommand, SqlParameter> { }
}

#endif