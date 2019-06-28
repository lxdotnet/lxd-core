
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using Model = Lxdn.Core.Basics.Model;

namespace Lxdn.Core.Expressions
{
    public class TypeBrowser
    {
        public TypeBrowser(IEnumerable<Model> models)
        {
            this.FriendlyAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(asm => 
                models.Any(m => m.Type.Assembly.GetName().Name.StartsWith(asm.GetName().Name.Split('.').First()))).ToList();
        }

        public IEnumerable<Type> KnownTypes
        {
            get
            {
                return AppDomain.CurrentDomain.GetAssemblies().OrderByDescending(asm => 
                    this.FriendlyAssemblies.Any(friendly => friendly == asm)).SelectMany(asm =>
                {
                    try
                    {
                        return asm.GetTypes();
                    }
                    catch
                    {
                        return Enumerable.Empty<Type>();
                    }
                });
            }
        }

        public IEnumerable<Assembly> FriendlyAssemblies { get; private set; }
    }
}
