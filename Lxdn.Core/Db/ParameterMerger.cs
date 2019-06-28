using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Db
{
    internal class ParameterMerger
    {
        public static TParameter[] Merge<TParameter>(IEnumerable<TParameter> input, IEnumerable<TParameter> output)
            where TParameter : DbParameter, new()
        {
            Func<IEnumerable<TParameter>, TParameter> merge = parameters =>
            {
                if (parameters.Count() == 1)
                    return parameters.Single();

                var output1 = parameters.Single(p => p.Direction == ParameterDirection.Output);
                var input1 = parameters.Single(p => p.Direction == ParameterDirection.Input);

                return new TParameter
                {
                    ParameterName = input1.ParameterName,
                    DbType = output1.DbType, // check
                    Value = input1.Value,
                    Direction = ParameterDirection.InputOutput,
                    Size = output1.Size
                };
            };

            var merged = input
                .Concat(output)
                .GroupBy(parameter => parameter.ParameterName, StringComparer.InvariantCultureIgnoreCase)
                .ThrowIf(groups => groups.Any(group => group.Count() > 2), group => new InvalidOperationException("Found more than 2 parameters having the same name"))
                .Select(merge).ToArray();

            return merged;
        }
    }
}