using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Lxd.Core.Expressions.Exceptions;
using Lxd.Core.Expressions.Operators.Models;

namespace Lxd.Core.Expressions.Operators
{
    public class MethodCall : Operator
    {
        private readonly Func<Expression> create;

        public MethodCall(MethodCallModel call, ExecutionEngine logic)
        {
            this.create = () =>
            {
                var arguments = call.Arguments.Select(a => logic.Operators.CreateFrom(a)).ToList();
                //.Select(op => op.Expression).ToList();

                // two opportunities for target: 
                //  - a type (and therefore a static function call)
                //  - an instance (and therefore an instance function call)
                // look if a type was referenced instead of property 
                Type type = AppDomain.CurrentDomain.GetAssemblies() // todo: get rid of constants below; consider rather all assemblies, do proper exception handling
                    .Where(asm => Regex.IsMatch(asm.FullName, @"^mscorlib|^System|^AUTOonline|^AXO")) // I restricted possible types for static calls because it results in type locaing exception for some 2rd party dlls loaded in the domain
                    .SelectMany(asm =>
                    {
                        try
                        {
                            return asm.GetTypes(); // there are some assemblies, that type retrieval fails with security exception
                        }
                        catch
                        {
                            return Enumerable.Empty<Type>();
                        }

                    })
                    .FirstOrDefault(t => t.FullName.Equals(call.Target));

                if (type != null) // static
                {
                    MethodInfo method = type.GetMethod(call.Method, BindingFlags.Static | BindingFlags.Public, null,
                        arguments.Select(arg => arg.Expression.Type).ToArray(), null);

                    if (method == null)
                        //this.Throw(CallType.Static, model.Method, type, arguments);
                    {
                        // try to pick up the method having the same count of parameter, later providing conversion if type does not match:
                        method = type.GetMethod(call.Method);

                        if (method == null || method.GetParameters().Length != arguments.Count)
                        {
                            Throw(CallType.Static, call.Method, type, arguments);
                        }
                    }

                    arguments = arguments
                        .Zip(method.GetParameters(), (arg, parameter) => new { Argument = arg, Needed = parameter })
                        .Select(mapping => // provide conversion for parameters, whose actual types differ from those in the method signature
                            mapping.Argument.Expression.Type == mapping.Needed.ParameterType
                                ? mapping.Argument
                                : mapping.Argument.As(mapping.Needed.ParameterType))//new AnyTypeConverter(mapping.Argument, mapping.Needed.ParameterType, logic))
                        .ToList();

                    return Expression.Call(method, arguments.Select(op => op.Expression));
                }
                else // instance
                {
                    Property p = logic.Operators.CreateProperty(call.Target);

                    MethodInfo method = p.Type.GetMethod(call.Method, BindingFlags.Instance | BindingFlags.Public, null,
                        arguments.Select(arg => arg.Expression.Type).ToArray(), null); // todo: reproduce the logic as above; 

                    if (method == null)
                        Throw(CallType.Instance, call.Method, p.Type, arguments);

                    return Expression.Call(p.Expression, method, arguments.Select(arg => arg.Expression));
                }
            };
        }

        private enum CallType // not used in the logic, just for the sake of beau
        {
            Static, Instance
        }

        static void Throw(CallType callType, string method, Type type, IList<Operator> arguments)
        {
            string message = string.Format("No {0} function with name {1} found in the type {2}",
                callType.ToString().ToLowerInvariant(), method, type.FullName);

            if (arguments.Count != 0)
                message = message + " having the following argument types: " +
                    string.Join(", ", arguments.Select(arg => arg.Expression.Type.FullName).ToArray());

            throw new ExpressionConfigException(message);
        }

        protected internal override Expression Create()
        {
            return this.create();
        }
    }
}
