using System;
using System.Reflection;
using Lxdn.Core.Validation;

namespace Lxdn.Core.Expressions.Operators.Models.Validation.Extensions
{
    internal static class Extensions
    {
        public static ValidationError ToError(this GenericMessages value)
        {
            var name = Enum.GetName(typeof(GenericMessages), value);
            var message = typeof(GenericMessages).GetMember(name)[0].GetCustomAttribute<MessageAttribute>();
            return new ValidationError(name, message != null ? message.Text : name);
        }
    }
}