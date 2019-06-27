
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Lxd.Core.Expressions.Operators.Custom;
using Lxd.Core.Expressions.Operators.Models.Strings;

namespace Lxd.Core.Expressions.Operators
{
    public class StringOccurence : CustomOperator<bool, IRuntimeContext>
    {
        private delegate bool Occurs(string source, string value);

        private readonly Func<IRuntimeContext, bool> occurs;

        private static readonly Dictionary<StringOccurenceKind, Occurs> Functions = new Dictionary<StringOccurenceKind, Occurs>();
        
        public StringOccurence(StringOccurenceModel model, ExecutionEngine logic) : base(logic)
        {
            if (!model.Occurence.HasValue)
                throw new InvalidOperationException("Occurence must be specified");

            var source = logic.Create<string>(model.Source);
            var value = logic.Create<string>(model.Value);

            if (Functions.ContainsKey(model.Occurence.Value))
                this.occurs = runtime => Functions[model.Occurence.Value](runtime.Resolve(source), runtime.Resolve(value));
            else
                throw new NotImplementedException("Not implemented for " + model.Occurence);
        }

        protected override bool Evaluate(IRuntimeContext runtime)
        {
            return this.occurs(runtime);
        }

        static StringOccurence()
        {
            Functions.Add(StringOccurenceKind.Contains, (source, value) => 
                -1 != Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(source, value, CompareOptions.IgnoreCase));

            Functions.Add(StringOccurenceKind.StartsWith, (source, value) =>
                source.StartsWith(value, StringComparison.CurrentCultureIgnoreCase));

            Functions.Add(StringOccurenceKind.EndsWith, (source, value) => 
                source.EndsWith(value, StringComparison.CurrentCultureIgnoreCase));

            Functions.Add(StringOccurenceKind.Matches, (source, value) =>
                Regex.IsMatch(source, value, RegexOptions.IgnoreCase));
        }
    }
}
