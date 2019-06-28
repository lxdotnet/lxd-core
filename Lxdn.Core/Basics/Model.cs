using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Lxdn.Core.Extensions;

namespace Lxdn.Core.Basics
{
    [DebuggerDisplay("{Type.Name, nq} {Id, nq}")]
    public class Model
    {
        public Model(string id, Type type)
        {
            this.Id = id;
            this.Type = type;

            this.parameter = Expression.Parameter(type, id);
        }

        public string Id { get; private set; }

        public Type Type { get; private set; }

        private readonly ParameterExpression parameter;

        public ParameterExpression AsParameter() // because it doesn't belong to the Model's model, so I don't wanna see it in properties, even in the debugger
        {
            return this.parameter;
        }

        private static object Populate(object model)
        {
            model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(prop => prop.PropertyType.IsClass) // todo: attention here, may be insufficient
                .Where(prop => prop.PropertyType.FindCtorAccepting(Enumerable.Empty<Type>()).HasValue) // require parameterless ctor
                .Where(prop => prop.HasPublicGetter() && prop.HasPublicSetter())
                .ToList().ForEach(prop =>
                {
                    var instance = Activator.CreateInstance(prop.PropertyType);
                    prop.SetValue(model, instance);

                    Populate(instance);
                });

            return model;
        }

        //public ModelInstance Instantiate()
        //{
        //    var instance = (ModelInstance)Activator.CreateInstance(this.Type);
        //    instance.Model = this;

        //    //return (ModelInstance)Populate(instance); // automatically create properties that we can create (recursively)
        //    return instance; // temporary, todo: test the population
        //}
    }
}
