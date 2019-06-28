using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Lxdn.Core.Expressions.Utils
{
    public class StructuredPath
    {
        public StructuredPath(string path)
        {
            Match match = Splitter.Match(path);
            if (string.IsNullOrEmpty(match.Value))
                throw new ArgumentException(string.Format("Invalid path: '{0}'.", path));

            this.Entry = match.Groups["entry"].Value;
            this.Values = new ReadOnlyCollection<string>(new List<string>(
                match.Groups["path"].Value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)));
        }

        internal StructuredPath(string entry, IList<string> values)
        {
            this.Entry = entry;
            this.Values = new ReadOnlyCollection<string>(values);
        }

        private static readonly Regex Splitter = new Regex(@"^(?<entry>.*?)(?<path>\..*)?$");

        public readonly string Entry;

        public readonly ReadOnlyCollection<string> Values;

        public override string ToString()
        {
            List<string> entries = new List<string>();
            entries.Add(this.Entry);
            entries.AddRange(this.Values);
            return string.Join(".", entries.ToArray());
        }
    }
}
