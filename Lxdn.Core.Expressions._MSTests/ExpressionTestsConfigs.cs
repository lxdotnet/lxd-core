
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

namespace Lxdn.Core.Expressions._MSTests
{
    internal class ExpressionTestsConfigs
    {
        public ExpressionTestsConfigs(string filename)
        {
            var tests = new XmlDocument();

            var bin = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
            tests.Load(Path.Combine(bin, filename));

            foreach (XmlNode node in tests.SelectNodes("/Tests/Test"))
            {
                this.configs.Add(node.Attributes["id"].Value, node.SelectSingleNode(@"./*"));
            }
        }

        public XmlNode Of(string id)
        {
            return this.configs[id];
        }

        private readonly Dictionary<string, XmlNode> configs = new Dictionary<string, XmlNode>();
    }
}
