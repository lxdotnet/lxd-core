
using System;
using System.Linq;
using System.Dynamic;
using System.ComponentModel;

using Lxdn.Core.Extensions;

namespace Lxdn.Core.Basics
{
    public class DynamicNotifyPropertyChanged : CaseInsensitiveExpando, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var success = base.TrySetMember(binder, value);
            FireChanged(binder.Name);
            return success;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var success = base.TrySetIndex(binder, indexes, value);

            var name = indexes
                .ThrowIf(x => x.Length != 1, x => new ArgumentException($"Unexpected count of indices: {x.Length}. Expected 1."))
                .Single().IfExists(x => x as string)
                .ThrowIfDefault(() => new ArgumentException($"The index being applied is null or not a string"));

            FireChanged(name);
            return success;
        }

        protected void FireChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

 