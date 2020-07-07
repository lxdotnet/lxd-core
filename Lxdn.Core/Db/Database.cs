using System;
using System.Data;
using System.Threading;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Lxdn.Core.Observables;

namespace Lxdn.Core.Db
{
    public class Database<TConnection, TBehaviors> : IDatabase
        where TConnection : DbConnection, new()
        where TBehaviors : IDatabaseFactory, new()
    {
        protected readonly Schema schema;

        private readonly Func<DbConnection, IDatabase> databaseBehind;

        private static readonly TBehaviors Behaviors = new TBehaviors();

        protected Database(Schema schema)
        {
            this.schema = schema;
            this.databaseBehind = Behaviors.Create;
        }

        public Task Transact(Func<IDatabase, Task> logic, IsolationLevel isolation = IsolationLevel.Unspecified)
        {
            return Transact(async db => {
                await logic(db);
                return 0;
            }, isolation);
        }           

        public async Task<TReturn> Transact<TReturn>(Func<IDatabase, Task<TReturn>> logic, IsolationLevel isolation = IsolationLevel.Unspecified)
        {
            using (var cn = new TConnection { ConnectionString = this.schema.ConnectionString })
            {
                await cn.OpenAsync().ConfigureAwait(false);

                using (var transaction = cn.BeginTransaction(isolation))
                {
                    try
                    {
                        var result = await logic(databaseBehind(cn)).ConfigureAwait(false);
                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<IReadOnlyCollection<TEntity>> Fetch<TEntity>(string sql, object parameters = null,
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TEntity : class, new()
        {
            using (var cn = new TConnection { ConnectionString = schema.ConnectionString })
            {
                return await Guard.Task(() => databaseBehind(cn).Fetch<TEntity>(sql, parameters, commandType, cancel), 
                    ex => new DatabaseException(schema, parameters, ex));
            }
        }

        public async Task<int> Execute(string sql, object parameters = null,
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
        {
            using (var cn = new TConnection { ConnectionString = schema.ConnectionString })
            {
                return await Guard.Task(() => databaseBehind(cn).Execute(sql, parameters, commandType, cancel),
                    ex => new DatabaseException(schema, parameters, ex));
            }
        }

        public Observable<TEntity> Observe<TEntity>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text) 
            where TEntity : class, new()
        {
            var cn = new TConnection { ConnectionString = schema.ConnectionString };
            return databaseBehind(cn).Observe<TEntity>(sql, parameters, commandType)
                .Subscribe(null, () => cn.Dispose());
        }

        public async Task<TReturn> ExecuteScalar<TReturn>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken)) where TReturn : struct
        {
            using (var cn = new TConnection { ConnectionString = schema.ConnectionString })
            {
                return await Guard.Task(() => databaseBehind(cn).ExecuteScalar<TReturn>(sql, parameters, commandType, cancel),
                    ex => new DatabaseException(schema, parameters, ex));
            }
        }

        public async Task<TReturn> ExecuteReturning<TReturn>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken)) where TReturn : class, new()
        {
            using (var cn = new TConnection { ConnectionString = schema.ConnectionString })
            {
                return await Guard.Task(() => databaseBehind(cn).ExecuteReturning<TReturn>(sql, parameters, commandType, cancel),
                    ex => new DatabaseException(schema, parameters, ex));
            }
        }
    }
}