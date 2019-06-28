using System.Data.Common;

namespace Lxd.Core.Db
{
    public interface IDatabaseFactory
    {
        IDatabase Create(DbConnection connection);
    }
}