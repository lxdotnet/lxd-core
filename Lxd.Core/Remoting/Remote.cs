
namespace Lxd.Core.Remoting
{

#if NETFULL // see https://stackoverflow.com/questions/27266907/no-appdomains-in-net-core-why

    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Replicates primary appdomain and creates ar remote instance of TLogic in new domain.
    /// TLogic is expected (but not required) to be ILogic&lt;TContext&gt;, 
    /// both TLogic, TContext must inherit from MarshalByRefObject.
    /// TContext contains members used as in- and out-parameters for the remote logic, they all must be at least [Serializable]
    /// some additional discussion on marshaling s. http://stackoverflow.com/questions/6339469/object-has-been-disconnected-or-does-not-exist-at-the-server-exception 
    /// </summary>
    /// <typeparam name="TLogic"></typeparam>
    public class Remote<TLogic> where TLogic : MarshalByRefObject
    {
        private readonly string domainName;

        private AppDomain domain;

        public Remote(string domainName)
        {
            this.domainName = domainName;
            this.Initialize();
        }

        private void setupDomain()
        {
            if (this.domain != null)
                AppDomain.Unload(this.domain);

            var current = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var setup = new AppDomainSetup { ApplicationBase = new FileInfo(current.LocalPath).DirectoryName };
            this.domain = AppDomain.CreateDomain(domainName, Assembly.GetExecutingAssembly().Evidence, setup);
            Assembly.ReflectionOnlyLoadFrom(current.LocalPath).GetReferencedAssemblies().ToList()
                .ForEach(assembly => this.domain.Load(assembly));
        }

        public TLogic Logic { get; private set; }

        public void Initialize()
        {
            this.setupDomain();

            var handle = this.domain.CreateInstance(typeof(TLogic).Assembly.FullName, typeof(TLogic).FullName, false,
                BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.Instance, null, /*this.arguments*/null, null, null);
            this.Logic = (TLogic)handle.Unwrap();
        }
    }

#endif
}

// about AccessViolationException and other corrupted state exception see
// https://msdn.microsoft.com/en-us/magazine/dd419661.aspx#id0070035