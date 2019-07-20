
using System;
using System.Reflection;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class StepFactory
    {
        private readonly Func<string, Step> createStep;

        private StepFactory(Type current)
        {
            createStep = token => {
                var property = current.GetProperty(token, BindingFlags.Public | BindingFlags.Instance)
                    .ThrowIfDefault(() => new ArgumentException($"Invalid token: {token}"));

                return new Step(property);
            };
        }

        public static StepFactory Of(Type type) => new StepFactory(type);

        public Step CreateStep(string token) => createStep(token);
    }
}