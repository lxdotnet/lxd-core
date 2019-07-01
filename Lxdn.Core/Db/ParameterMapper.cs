using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Lxdn.Core.Extensions;
using Lxdn.Core.Injection;

namespace Lxdn.Core.Db
{
    internal class ParameterMapper<TParameter>
        where TParameter : DbParameter
    {
        private readonly Func<TParameter, IParameterFacade> createFacade;

        public ParameterMapper(Func<TParameter, IParameterFacade> createFacade)
        {
            this.createFacade = createFacade;
        }

        internal TReturn MapTo<TReturn>(DbParameterCollection parameters)
            where TReturn: class , new()
        {
            return Enumerable.Range(0, parameters.Count)
                .Select(ordinal => (TParameter)parameters[ordinal])
                .Where(parameter => parameter.Direction.IsOneOf(ParameterDirection.Output, ParameterDirection.InputOutput))
                .Where(parameter => parameter.Value != DBNull.Value)
                .Select(createFacade)
                .Where(parameter => !parameter.IsNull)
                .ToDynamic(parameter => parameter.Name, parameter => parameter.Value)
                .InjectTo<TReturn>();
        }
    }
}