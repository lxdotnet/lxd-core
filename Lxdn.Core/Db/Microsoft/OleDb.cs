#if NETFULL

using System.Data.OleDb;

namespace Lxdn.Core.Db.Microsoft
{
    public class OleDb : Database<OleDbConnection, OleDbBehaviors>
    {
        public OleDb(Schema schema) : base(schema) { }

        public static IDatabase Use(Schema schema) => new OleDb(schema);
    }

    public class OleDbBehaviors : DbBehaviors<OleDbCommand, OleDbParameter> { }
}

#endif
