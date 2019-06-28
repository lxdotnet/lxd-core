
using System;

namespace Lxd.Core.Extensions
{
    public static class SystemExtensions
    {
        public static Decimal StripNonsignificantZeros(this Decimal number)
        {
            var bits = Decimal.GetBits(number); // http://msdn.microsoft.com/en-us/library/system.decimal.getbits(v=vs.110).aspx
            var degree = (bits[3]/*flags*/ & 0xff0000) >> 0x10; 

            if (degree > 0) // has something after comma 
            {
                var remainder = bits[0] /*lo*/% Convert.ToInt32(Math.Pow(10, degree));
                if (remainder == 0)
                    return Math.Round(number); // eliminates zeros after comma
            }

            return number;
        }

        public static bool Negate(this bool value)
        {
            return !value;
        }
    }
}
