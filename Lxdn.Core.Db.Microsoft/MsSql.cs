﻿
// MSSQL out of the box is currently available for .net classic only
// to add the support for .net core see https://stackoverflow.com/questions/35444487/how-to-use-sqlclient-in-asp-net-core

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