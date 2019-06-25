using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lxd.Core.Extensions;

namespace Lxd.Core.Db
{
    public static class Extensions
    {
        public static TEntity To<TEntity>(this IDataRecord record)
            where TEntity : class, new()
        {
            var values = Enumerable.Range(0, record.FieldCount)
                .Where(field => !record.IsDBNull(field))
                .ToDictionary(record.GetName, record.GetValue, StringComparer.OrdinalIgnoreCase);

            var result = typeof(TEntity) == typeof(object) // dynamic requested
                ? (dynamic)values.ToDynamic()
                : values.To<TEntity>();

            return result;
        }

        public static async Task<TCommand> Connect<TCommand>(this TCommand command, CancellationToken cancel)
            where TCommand: DbCommand
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync(cancel).ConfigureAwait(false);
            }

            return command;
        }

        public static Task<TCommand> Connect<TCommand>(this TCommand command) where TCommand : DbCommand 
            => command.Connect(CancellationToken.None);
    }
}