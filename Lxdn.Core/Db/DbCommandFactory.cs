using System;
using System.Data;
using System.Data.Common;
using Lxd.Core.Extensions;

namespace Lxd.Core.Db
{
    public class DbCommandFactory<TCommand, TParameter>
        where TCommand : DbCommand, new()
        where TParameter : DbParameter, new()
    {
        private readonly DbBehaviors<TCommand, TParameter> behaviors;

        public DbCommandFactory(DbBehaviors<TCommand, TParameter> behaviors)
        {
            this.behaviors = behaviors;
        }

        public DbCommand CreateCommand(DbConnection connection, string sql, object parameters = null, CommandType commandType = CommandType.Text)
        {
            var command = new TCommand
            {
                Connection = connection.ThrowIfDefault(() => new ArgumentNullException(nameof(connection))),
                CommandText = sql.ThrowIf(string.IsNullOrEmpty, x => new ArgumentNullException(nameof(sql))),
                CommandType = commandType
            };

            behaviors.VisitorOfCommand.IfExists(visit => visit(command)); // assume not throws

            foreach (var property in parameters.ToDictionary())
            {
                try
                {
                    var parameter = new Parameter { Name = property.Key, Value = property.Value ?? DBNull.Value };

                    behaviors.VisitorOfParameter.IfExists(visit => visit(parameter));

                    command.Parameters.Add(new TParameter { ParameterName = parameter.Name, Value = parameter.Value });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating {typeof(TParameter).Name} for " + property.Key, ex);
                }
            }

            return command;
        }
    }
}