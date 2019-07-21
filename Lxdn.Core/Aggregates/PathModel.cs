using System.Linq;
using System.Collections.Generic;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class PathModel
    {
        public static PathModel Parse(string path)
        {
            var tokens = path.SplitBy(".").ToList();
            return new PathModel { Root = tokens[0], Tokens = tokens.Skip(1) };
        }

        public string Root { get; set; }

        public IEnumerable<string> Tokens { get; set; }
    }
}