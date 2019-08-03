using System;
using System.Linq;
using System.Data;
using System.Threading;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;

using Lxdn.Core.Extensions;
using Lxdn.Core.Observables;

namespace Lxdn.Core.Db
{
    /// <summary>
    /// Runs sql on a connection provided _externally_,
    /// therefore is (or can be) a part of unit of work
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TParameter"></typeparam>
    public class SqlRunner<TCommand, TParameter> : IDatabase
        where TCommand : DbCommand, new()
        where TParameter : DbParameter, new()
    {
        private readonly DbConnection connection;

        private readonly DbBehaviors<TCommand, TParameter> behaviors;

        private readonly DbCommandFactory<TCommand, TParameter> factory;

        protected internal SqlRunner(DbConnection connection, DbBehaviors<TCommand, TParameter> behaviors)
        {
            this.connection = connection;
            this.behaviors = behaviors;

            this.factory = new DbCommandFactory<TCommand, TParameter>(behaviors);
        }

        public async Task<IReadOnlyCollection<TEntity>> Fetch<TEntity>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TEntity : class, new()
        {
            using (var command = await factory.CreateCommand(connection, sql, parameters, commandType).Connect(cancel))
            using (var reader = await command.ExecuteReaderAsync(cancel))
            {
                return reader.OfType<IDataRecord>()
                    .Select(record => record.To<TEntity>())
                    .Aggregate(new List<TEntity>(), (entities, entity) => entities.Push(entity))
                    .ToList().AsReadOnly();
            }
        }

        public async Task<int> Execute(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
        {
            using (var command = await factory.CreateCommand(connection, sql, parameters, commandType).Connect(cancel))
            {
                return await command.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);
            }
        }

        public Observable<TEntity> Observe<TEntity>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text)
            where TEntity : class, new()
        {
            return Observable<TEntity>.Create(async (observer, cancel) =>
            {
                try
                {
                    using (var command = await factory.CreateCommand(connection, sql, parameters, commandType).Connect(cancel))
                    using (var reader = await command.ExecuteReaderAsync(cancel).ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            observer.OnNext(reader.To<TEntity>());
                        }
                    }
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }
                finally
                {
                    observer.OnCompleted(); // or should be either OnCompleted or OnError but not both?
                }
            });
        }

        public async Task<TReturn> ExecuteScalar<TReturn>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TReturn : struct
        {
            using (var command = await factory.CreateCommand(connection, sql, parameters, commandType).Connect(cancel))
            {
                return (await command.ExecuteScalarAsync(cancel).ConfigureAwait(false)).ChangeType<TReturn>();
            }
        }

        public async Task<TReturn> ExecuteReturning<TReturn>(string sql, object parameters = null, 
            CommandType commandType = CommandType.Text, CancellationToken cancel = default(CancellationToken))
            where TReturn : class, new()
        {
            using (var command = factory.CreateCommand(connection, sql, parameters, commandType))
            {
                var output = typeof(TReturn).GetProperties()
                    .Select(property => new TParameter
                    {
                        ParameterName = property.Name,
                        DbType = property.PropertyType.ToDbType() ?? DbType.String,
                        Direction = ParameterDirection.Output,
                        Size = BufferSize.Of(property.PropertyType) ?? 0
                    })
                    .ToList();

                var merged = ParameterMerger.Merge(command.Parameters.Cast<TParameter>(), output);

                command.Parameters.Clear();
                command.Parameters.AddRange(merged);

                await command.Connect(cancel);
                await command.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                var mapper = new ParameterMapper<TParameter>(behaviors.CreateFacade
                    .ThrowIfDefault(() => new NotImplementedException("Parameter facade not specified")));

                return mapper.MapTo<TReturn>(command.Parameters);
            }
        }
    }
}
