
using System;
using System.Linq;
using System.Reflection;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Aggregates
{
    public class StepFactory
    {
        private readonly Func<string, Step> createStep;

        private static readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private StepFactory(Type current)
        {
            PropertyInfo propertyOf(Type type, string token) => type.GetProperty(token, flags)
                    ?? type.GetInterfaces() // type may implement a property declared in some interface. In this case just .GetProperty returns null.
                        .SelectMany(interfaces => interfaces.GetProperties(flags))
                        .FirstOrDefault(i => i.Name.Equals(token));

            createStep = token => {
                var property = propertyOf(current, token)
                    .ThrowIfDefault(() => new ArgumentException($"Invalid token: {token}"));

                return new Step(property);
            };
        }

        public static StepFactory Of(Type type) => new StepFactory(type);

        public Step CreateStep(string token) => createStep(token);
    }
}