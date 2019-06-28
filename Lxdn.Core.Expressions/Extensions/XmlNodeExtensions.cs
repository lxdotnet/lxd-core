
using System;
using System.Xml;
using System.Xml.Linq;

using Lxd.Core.Extensions;

namespace Lxd.Core.Expressions.Extensions
{
    public class XmlConfigException : ApplicationException
    {
        public XmlConfigException(string message) : base(message) { }
        public XmlConfigException(string message, Exception inner) : base(message, inner) { }
    }

    internal static class XmlNodeExtensions
    {
        private static TReturn GetAttribute<TReturn>(this XmlNode xml, string name, bool throwIfNoFound)
        {
            XmlNode attribute = xml.Attributes[name];
            if (null == attribute || string.IsNullOrEmpty(attribute.Value))
            {
                if (throwIfNoFound)
                    throw new XmlConfigException("Mandatory attribute '" + name + "' missing.");

                return default(TReturn);
            }

            try
            {
                return attribute.Value.To<TReturn>();
            }
            catch (Exception ex)
            {
                throw new XmlConfigException(string.Format("Cannot cast '{0}' to the type {1}", attribute.Value, typeof(TReturn)), ex);
            }
        }

        public static TReturn GetAttributeOrDefault<TReturn>(this XmlNode xml, string name)
        {
            return GetAttribute<TReturn>(xml, name, false);
        }

        public static TReturn GetMandatoryAttribute<TReturn>(this XmlNode xml, string name)
        {
            return GetAttribute<TReturn>(xml, name, true);
        }

        public static string GetAttributeOrDefault(this XmlNode xml, string name)
        {
            return GetAttribute<string>(xml, name, false);
        }

        public static string GetMandatoryAttribute(this XmlNode xml, string name)
        {
            return GetAttribute<string>(xml, name, true);
        }

        public static XmlNode GetMandatoryNode(this XmlNode xml, string path)
        {
            var node = xml.SelectSingleNode(path);
            if (node == null)
                throw new XmlConfigException("Missing mandatory node " + path);

            return node;
        }

        /// <summary>
        /// Based on http://blogs.msdn.com/b/ericwhite/archive/2008/12/22/convert-xelement-to-xmlnode-and-convert-xmlnode-to-xelement.aspx
        /// </summary>
        /// <param name="node"></param>
        public static XElement ToXElement(this XmlNode node)
        {
            XDocument document = new XDocument();
            using (XmlWriter writer = document.CreateWriter())
            {
                node.WriteTo(writer);
                return document.Root;
            }
        }

        public static XmlNode ToXmlNode(this XElement element)
        {
            using (XmlReader reader = element.CreateReader())
            {
                XmlDocument document = new XmlDocument();
                document.Load(reader);
                return document.SelectSingleNode(@"./*"); // root
            }
        }
    }
}
