using System;
using Lxd.Core.Extensions;

namespace Lxd.Annotations
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly Func<Type, object> resolver;

        public ServiceProvider(Func<Type, object> resolve)
        {
            this.resolver = resolve;
        }

        public object GetService(Type serviceType)
        {
            return this.resolver.IfExists(resolve => resolve(serviceType));
        }
    }
}