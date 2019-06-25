using System;
using System.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

using Lxd.Core.Basics;

namespace Lxd.Core.Extensions
{
    public static class TypeExtensions
    {
        public static string CamelizeName(this Type type)
        {
            return char.ToLowerInvariant(type.Name[0]) + type.Name.Substring(1);
        }

        public static object DefaultValue(this Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        //public static MayBe<Type> AsEnumerableOf(this Type type)
        //{
        //    if (type.IsGenericType && typeof(IEnumerable<>) == type.GetGenericTypeDefinition())
        //        return new MayBe<Type>(type.GetGenericArguments().First());

        //    Type candidate = type.GetInterfaces().FirstOrDefault(iface =>
        //        iface.IsGenericType &&
        //        typeof(IEnumerable<>) == iface.GetGenericTypeDefinition());

        //    if (candidate != null)
        //        return new MayBe<Type>(candidate.GetGenericArguments().First());

        //    return MayBe<Type>.Nothing;
        //}

        /// <summary>
        /// When has value, the value is the underlying type of Nullable
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MayBe<Type> AsNullableOf(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return new MayBe<Type>(type.GetGenericArguments()[0]);

            return MayBe<Type>.Nothing;
        }

        public static MayBe<ConstructorInfo> FindCtorAccepting(this Type type, IEnumerable<Type> argTypes)
        {
            ConstructorInfo ci = type.GetConstructors().FirstOrDefault(ctor =>
                ctor.GetParameters().Count() == argTypes.Count()
                && ctor.GetParameters()
                    .Zip(argTypes, (param, argType) => new { Parameter = param, ArgumentType = argType })
                    .All(pair => pair.Parameter.ParameterType.IsAssignableFrom(pair.ArgumentType)));

            return ci != null ? new MayBe<ConstructorInfo>(ci) : MayBe<ConstructorInfo>.Nothing;
        }

        public static MayBe<ConstructorInfo> FindCtorAccepting(this Type type, Type arg1)
        {
            return type.FindCtorAccepting(new List<Type> { arg1 });
        }

        public static MayBe<ConstructorInfo> FindCtorAccepting(this Type type, Type arg1, Type arg2)
        {
            return type.FindCtorAccepting(new List<Type> { arg1, arg2 });
        }

        public static MayBe<ConstructorInfo> FindCtorAccepting(this Type type, Type arg1, Type arg2, Type arg3)
        {
            return type.FindCtorAccepting(new List<Type> { arg1, arg2, arg3 });
        }

        public static MayBe<ConstructorInfo> FindCtorAccepting(this Type type, IEnumerable<object> args)
        {
            return type.FindCtorAccepting(args.Select(arg => arg.GetType()));
        }

        public static Type FindParent<TParent>(this Type type)
        {
            return type.FindParent(typeof(TParent));
        }

        public static Type FindParent(this Type type, Type parent)
        {
            if (type.BaseType == null)
                return null;

            if (parent == type.BaseType)
                return type.BaseType;

            if (type.BaseType.IsGenericType)
            {
                Type g = type.BaseType.GetGenericTypeDefinition();
                if (parent == g)
                    return type.BaseType;
            }

            return type.BaseType.FindParent(parent);
        }

        public static IReadOnlyCollection<Type> GetAncestors(this Type type)
        {
            Stack<Type> hierarchy = new Stack<Type>();
            Type current = type;

            while (current.BaseType != null)
            {
                hierarchy.Push(current);
                current = current.BaseType;
            }

            return hierarchy.ToList().AsReadOnly();
        }

        //public static int DistanceTo(this Type @base, Type derived)

        /// <summary>
        /// http://stackoverflow.com/questions/1124563/builds-a-delegate-from-methodinfo
        /// </summary>
        /// <param name="method"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Delegate ToDelegate(this MethodInfo method, object target)
        {
            if (method == null) throw new ArgumentNullException("method");

            Type delegateType;

            var typeArgs = method.GetParameters()
                .Select(p => p.ParameterType)
                .ToList();

            // builds a delegate type
            if (method.ReturnType == typeof(void))
            {
                delegateType = Expression.GetActionType(typeArgs.ToArray());
            }
            else
            {
                typeArgs.Add(method.ReturnType);
                delegateType = Expression.GetFuncType(typeArgs.ToArray());
            }

            // creates a bound delegate if target was supplied
            var result = (target == null)
                ? Delegate.CreateDelegate(delegateType, method)
                : Delegate.CreateDelegate(delegateType, target, method);

            return result;
        }

        public static PropertyInfo GetIndexer(this Type type, params Type[] arguments)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                //.Where(property => property.HasPublicGetter())
                .Select(property => new
                {
                    Value = property,
                    IndexTuples = property
                        .GetIndexParameters()
                        .Select(p => p.ParameterType)
                        .Zip(arguments, (existing, required) => new { Existing = existing, Required = required })
                })
                .First(property => (property.IndexTuples.Any() &&
                                    property.IndexTuples.All(tuple => tuple.Existing == tuple.Required)))
                .Value;
        }

        private static readonly Dictionary<Type, IEqualityComparer> Comparers = new Dictionary<Type, IEqualityComparer>();

        internal class DefaultComparer : IEqualityComparer
        {
            internal DefaultComparer(Type type)
            {
                this.type = type;
                this.comparer = (IEqualityComparer)typeof(EqualityComparer<>).MakeGenericType(type).GetProperty("Default").GetValue(null);
            }

            private readonly IEqualityComparer comparer;

            private readonly Type type;

            bool IEqualityComparer.Equals(object x, object y)
            {
                if (x.GetType() != this.type)
                    return false;

                if (y.GetType() != this.type)
                    return false;

                return this.comparer.Equals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return this.comparer.GetHashCode(obj);
            }
        }

        public static IEqualityComparer CreateComparer(this Type type)
        {
            lock (Comparers)
            {
                if (!Comparers.ContainsKey(type))
                    Comparers.Add(type, new DefaultComparer(type));
                return Comparers[type];
            }
        }

        public static MayBe<IEnumerable<Type>> AsArgumentsOf(this Type type, Type generic)
        {
            //if (!type.IsInterface)
            if (true)
            {
                return type.GetInterfaces()
                    .Concat(type.Once())
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == generic)
                    .IfExists(concrete => new MayBe<IEnumerable<Type>>(concrete.GetGenericArguments())) ?? MayBe<IEnumerable<Type>>.Nothing;
            }

            //return MayBe<IEnumerable<Type>>.Nothing;
        }

        public static TItem Instantiate<TItem>(this Type type, params object[] arguments) 
            where TItem : class
        {
            return (Activator.CreateInstance(type, arguments) as TItem)
                .ThrowIfDefault(() => new InvalidOperationException("Not a " + typeof(TItem).FullName));
        }
    }
}