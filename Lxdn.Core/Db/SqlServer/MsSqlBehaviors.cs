#if NETFULL

using System.Data.SqlClient;

namespace Lxdn.Core.Db.SqlServer
{
    public class MsSqlBehaviors : DbBehaviors<SqlCommand, SqlParameter> { }
}

#endif