using System;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;

using Lxdn.Core.Basics;
using Lxdn.Core.Expressions.Exceptions;
using Lxdn.Core.Expressions.Operators.Models;
using Lxdn.Core.Expressions.Utils;

namespace Lxdn.Core.Expressions.Operators
{
    public class Property : Operator
    {
        private readonly PropertyModel property;

        private readonly ExecutionEngine logic;

        private readonly Lazy<PropertyAccessor> accessor;

        public Property(string path, ExecutionEngine engine) : this(new PropertyModel { Path = path }, engine) { }

        public Property(PropertyModel property, ExecutionEngine logic)
        {
            this.logic = logic;
            this.property = property;
            this.accessor = new Lazy<PropertyAccessor>(() => new PropertyAccessor(this.Expression, logic.Models.ToArray()));
        }

        protected internal override Expression Create()
        {
            var structuredPath = new StructuredPath(this.property.Path);

            Model root = this.logic.Models.FirstOrDefault(m => m.Id.Equals(structuredPath.Entry));
            if (root == null)
                throw new ExpressionConfigException("Unknown model referenced in the property: " + structuredPath.Entry);

            var resolver = new PropertyResolver(root.Type);
            var property = resolver.Resolve(structuredPath);

            return property.ToExpression(root.AsParameter());
        }

        public Property StepInto(string nested)
        {
            var path = Expression + "." + nested;
            var model = new PropertyModel { Path = path };

            return new Property(model, logic);
        }

        public Property StepInto(PropertyInfo nested) => StepInto(nested.Name);

        //public GenericProperty ToGeneric()
        //{
        //    return new GenericProperty(this, this.logic.Clone());
        //}

        //private Property(Property path, string nested, ExecutionEngine engine)
        //    : this(new PropertyModel(path.Expression + "." + nested), engine) {}

        public Type Type => this.Expression.Type;

        public PropertyInfo Native => (this.Expression as MemberExpression)?.Member as PropertyInfo;

        public string ToName()
        {
            return this.Expression.ToString().Replace(".", "");
        }

        public PropertyAccessor Accessor => this.accessor.Value;

        private Model GetParameter(Expression expression)
        {
            var parameter = expression as ParameterExpression;

            if (parameter != null)
                return this.logic.Models.First(candidate => candidate.Id.Equals(parameter.Name));

            var member = expression as MemberExpression;
            if (member != null)
                return this.GetParameter(member.Expression);

            throw new InvalidOperationException();
        }

        public Model Root => this.GetParameter(this.Expression);

        //// todo: obsolete?
        //public MayBe<Property> Resolve(Type target)
        //{
        //    var resolver = new ExpressionResolver(target);
        //    var expression = resolver.Resolve(this.Expression);

        //    var member = expression as MemberExpression;
        //    if (member != null)
        //    {
        //        var property = new Property(member, this.logic); // todo: attention! .Create call is missing
        //        return MayBe<Property>.FromValue(property);
        //    }

        //    var parameter = expression as ParameterExpression;
        //    if (parameter != null)
        //    {
        //        var property = new Property(parameter, this.logic); // todo: see above
        //        return MayBe<Property>.FromValue(property); 
        //    }

        //    return MayBe<Property>.Nothing;
        //}

        //public static implicit operator GenericProperty(Property property)
        //{
        //    return property.ToGeneric();
        //}
    }
}
