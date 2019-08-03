
using System;
using System.Linq;
using System.Reflection;

using Lxdn.Core.Extensions;
using Lxdn.Core.Aggregates.Models;

namespace Lxdn.Core.Aggregates
{
    public class StepFactory
    {
        private readonly Func<IStepModel, IStep> createStep;

        private static readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private StepFactory(Type current)
        {
            PropertyInfo propertyOf(string token) => current.GetProperty(token, flags)
                    ?? current.GetInterfaces() // type may implement a property declared in some interface. In this case just .GetProperty returns null.
                        .SelectMany(interfaces => interfaces.GetProperties(flags))
                        .FirstOrDefault(i => i.Name.Equals(token));

            createStep = model => 
            {
                switch (model)
                {
                    case PropertyModel step:
                        var property = propertyOf(step.Value)
                            .ThrowIfDefault(() => new ArgumentException($"Invalid token: {step.Value}"));
                        return new Step(property);

                    case IndexModel index:
                        return new Index(index.Value);

                    default: throw new NotSupportedException($"Unsupported model: {model}");
                }
            };
        }

        public static StepFactory Of(Type type) => new StepFactory(type);

        public IStep Create(IStepModel model) => createStep(model);
    }
}