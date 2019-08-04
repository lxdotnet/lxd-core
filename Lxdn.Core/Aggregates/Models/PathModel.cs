using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates.Models
{
    public class PathModel
    {
        private static readonly Regex x = new Regex($"^(?'property'.*?)(\\[(?'index'\\d+)\\])?$"); // 'foo[1]' => property = 'foo', index = 1

        public static PathModel Parse(string path)
        {
            var tokens = path.SplitBy(".").ToList();

            IEnumerable<IStepModel> stepsOf(string token)
            {
                var match = x.Match(token);
                yield return new PropertyModel { Value = match.Groups["property"].Value };

                var index = match.Groups["index"].Value;
                if (!string.IsNullOrEmpty(index))
                    yield return new IndexModel { Value = index.ChangeType<int>() };
            }

            var steps = tokens.Skip(1).Aggregate(new List<IStepModel>(), 
                (_steps, token) => _steps.PushMany(stepsOf(token)));

            return new PathModel { Root = tokens.First(), Steps = steps };
        }

        public string Root { get; set; }

        public IEnumerable<IStepModel> Steps { get; set; }
    }
}