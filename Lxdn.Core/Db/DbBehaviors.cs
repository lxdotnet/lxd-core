using System;
using System.Data.Common;

namespace Lxd.Core.Db
{
    public class DbBehaviors<TCommand, TParameter> : IDatabaseFactory
        where TCommand: DbCommand, new()
        where TParameter: DbParameter, new()
    {
        public Action<TCommand> VisitorOfCommand { get; set; }
        public Action<Parameter> VisitorOfParameter { get; set; }
        public Func<TParameter, IParameterFacade> CreateFacade { get; set; }

        public IDatabase Create(DbConnection cn) => new SqlRunner<TCommand, TParameter>(cn, this);
    }
}