using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Lxdn.Core.Observables;

namespace Lxdn.Core.Db
{
    public interface IDatabase
    {
        Task<IReadOnlyCollection<TEntity>> Fetch<TEntity>(string sql, object parameters = null,
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TEntity : class, new();

        Task<int> Execute(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken));

        Observable<TEntity> Observe<TEntity>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text)
            where TEntity : class, new();

        Task<TReturn> ExecuteScalar<TReturn>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TReturn : struct;

        Task<TReturn> ExecuteReturning<TReturn>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TReturn : class, new();
    }
}