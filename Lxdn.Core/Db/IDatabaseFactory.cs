using System.Data.Common;

namespace Lxdn.Core.Db
{
    public interface IDatabaseFactory
    {
        IDatabase Create(DbConnection connection);
    }
}