
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Lxdn.Core.Expressions
{
    // inspired by https://mhusseini.wordpress.com/2014/07/21/reflection-emit-and-type-inheritance-calling-base-type-constructors/
    // http://stackoverflow.com/questions/3862226/dynamically-create-a-class-in-c-sharp

    public class OperatorModelEmitter
    {
        private static readonly AssemblyBuilder DynamicAssembly;

        private static readonly ModuleBuilder Module;

        static OperatorModelEmitter()
        {
            string name = Assembly.GetExecutingAssembly().GetName().Name + ".Emit.dll";
            var assemblyName = new AssemblyName(name);
            DynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            Module = DynamicAssembly.DefineDynamicModule(name);
        }

        public static Type TryLoad(string className)
        {
            return DynamicAssembly.GetType(className, false);
        }

        public static Type DeriveFrom(Type @base, string derivedName)
        {
            const TypeAttributes attributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

            if (!derivedName.Contains("."))
                derivedName = @base.Namespace + "." + derivedName;

            var type = Module.DefineType(derivedName, attributes, @base);
            //var ctor = type.DefineDefaultConstructor(MethodAttributes.Public);
            foreach (var ctor in @base.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
            {
                var args = ctor.GetParameters().Select(parameter => parameter.ParameterType).ToArray();
                var ctorOfDerived = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, args);

                var il = ctorOfDerived.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // push 'this" as the ctor is a 'this'-call, so the first implicit argument is always this

                if (args.Length > 0)
                    il.Emit(OpCodes.Ldarg_1); // pass over the 1st ctor arg 

                if (args.Length > 1)
                    il.Emit(OpCodes.Ldarg_2); // pass over the 2nd ctor arg

                if (args.Length > 2)
                    il.Emit(OpCodes.Ldarg_3);

                if (args.Length > 3)
                    throw new NotSupportedException("Generation of constructors having more than 3 arguments is not supported");
                
                il.Emit(OpCodes.Call, @base.GetConstructor(args));
                il.Emit(OpCodes.Ret);
            }
            
            return type.CreateType();
        }
    }
}
