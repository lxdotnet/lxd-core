using Lxdn.Core.Basics;
using Lxdn.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lxdn.Core.Expressions.Utils
{
    public class PropertyResolver
    {
        public PropertyResolver(Type entry)
        {
            #region param validation

            if (null == entry)
                throw new ArgumentNullException("entry");

            #endregion

            this.Entry = entry;
        }

        public readonly Type Entry;

        public IProperty Resolve(string path)
        {
            return this.Resolve(new StructuredPath(path));
        }

        public IProperty Resolve(StructuredPath path)
        {
            return this.Resolve(path, true);
        }

        private void Resolve(StructuredPath path, bool @throw, ref PropertyAdapter root)
        {
            PropertyToken token = new PropertyToken(path.Values[0]);

            PropertyInfo property = this.Entry.GetProperty(token.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // try looking in the interfaces, if any:
            // (story: when .Entry is IList'1[...], .GetProperty(name) returns null)
            if (null == property)
            {
                property = this.Entry.GetInterfaces()
                    .SelectMany(itf => itf.GetProperties())
                    .FirstOrDefault(p => p.Name.Equals(token.Name));
            }

            #region property validation

            if (null == property)
            {
                if (@throw)
                    throw new ArgumentException(string.Format("The public property '{0}' referenced in the property path does not exist.", token.Name), "path");
            }

            if (!property.CanRead)
                throw new ArgumentException(string.Format("The property '{0}' has no public get-accessor.", token.Name), "path");

            PropertyEntry entry = token.HasIndex
                ? new PropertyEntry(property, token.Index)
                : new PropertyEntry(property);

            root.Track.With(entry);

            //if (0 != property.GetIndexParameters().Length)
            //    throw new ArgumentException(string.Format("The property '{0}' is indexed and not supported yet.", token.Name), "path");

            #endregion

            List<string> tokens = new List<string>(path.Values);
            tokens.RemoveAt(0);

            if (0 != tokens.Count)
            {
                //PropertyResolver recursive = new PropertyResolver(property.PropertyType);
                PropertyResolver recursive = new PropertyResolver(entry.Type);
                recursive.Resolve(new StructuredPath(property.Name, tokens), @throw, ref root);
            }
        }

        private IProperty Resolve(StructuredPath path, bool @throw)
        {
            if (null == path)
                throw new ArgumentNullException("path");

            if (0 == path.Values.Count)
                return new ObjectAdapter(this.Entry);

            PropertyAdapter root = new PropertyAdapter();
            this.Resolve(path, @throw, ref root);

            return root;
        }
    }
}
