
using System;

namespace Lxdn.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToOffset(this DateTime dateTime)
        {
            if (dateTime.Ticks == 0)
                return new DateTimeOffset();

            return dateTime.Kind == DateTimeKind.Unspecified
                ? new DateTime(dateTime.Ticks, DateTimeKind.Local).ToOffset()
                : new DateTimeOffset(dateTime.ToLocalTime());
        }
    }
}
