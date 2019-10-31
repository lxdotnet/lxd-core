
using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Reflection;
using System.Data.Common;
using System.Threading.Tasks;

using Lxdn.Core.Injection;
using Lxdn.Core.Extensions;


namespace Lxdn.Core.Db
{
    public static class Extensions
    {
        internal static TEntity To<TEntity>(this IDataRecord record)
            where TEntity : class, new()
        {
            var row = Enumerable.Range(0, record.FieldCount)
                .Where(field => !record.IsDBNull(field))
                .ToDictionary(record.GetName, record.GetValue, StringComparer.OrdinalIgnoreCase);

            object valueOf(PropertyInfo property) =>
                row.SafeValueOf(property.Name)?.ChangeType(property.PropertyType);

            var result = typeof(TEntity) == typeof(object) // dynamic requested
                ? (dynamic) row.ToDynamic()
                : new TEntity().Inject(valueOf);

            return result;
        }

        internal static async Task<TCommand> Connect<TCommand>(this TCommand command, CancellationToken cancel)
            where TCommand: DbCommand
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                await command.Connection.OpenAsync(cancel).ConfigureAwait(false);
            }

            return command;
        }

        internal static Task<TCommand> Connect<TCommand>(this TCommand command) where TCommand : DbCommand 
            => command.Connect(CancellationToken.None);
    }
}