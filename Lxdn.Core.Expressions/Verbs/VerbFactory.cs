
using System;
using System.Linq;

using Lxdn.Core.Expressions.Operators;
using Lxdn.Core.Expressions.Operators.Models.Domain;

namespace Lxdn.Core.Expressions.Verbs
{
    public class VerbFactory
    {
        private readonly ExecutionEngine execution;

        public VerbFactory(ExecutionEngine execution)
        {
            this.execution = execution;
        }

        public object Create(VerbModel model, BinaryStatement statement)
        {
            var verb = this.execution.Operators.Models.Sources
                .Select(source => source.Assembly)
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetInterfaces().Any(i => i.IsGenericType && typeof(IVerb<>) == i.GetGenericTypeDefinition()) && !type.IsAbstract)
                .SingleOrDefault(type => type.GetConstructors().Any(ctor => ctor.GetParameters().Any(p => p.ParameterType.IsInstanceOfType(model))));

            if (verb == null)
                throw new Exception("Cannot find verb for the model " + model.GetType().FullName);

            return Activator.CreateInstance(verb, model, statement);
        }
    }
}
