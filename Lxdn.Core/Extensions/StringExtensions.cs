using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Lxdn.Core.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<Match> MatchesOf(this string input, Regex regex)
        {
            #region param validation

            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException("input");

            if (null == regex)
                throw new ArgumentNullException("regex");

            #endregion

            Match match = regex.Match(input);
            while (match.Success)
            {
                yield return match;
                match = match.NextMatch();
            }
        }

        public static IEnumerable<Match> MatchesOf(this string input, string pattern) => input.MatchesOf(new Regex(pattern));

        public static string Replace(this string input, string pattern, MatchEvaluator evaluator)
            => input.IfExists(s => Regex.Replace(s, pattern, evaluator));

        //[Obsolete("Candidate for removal")]
        public static string ToLowerCamelCase(this string input)
        {
            string[] tmp = input.Split('.');
            for (int i = 0; i < tmp.Length; i++)
            {
                tmp[i] = Char.ToLowerInvariant(tmp[i][0]) + tmp[i].Substring(1);
            }
            return String.Join(".", tmp);
        }

        public static T To<T>(this string s, CultureInfo culture) => (T)s.To(typeof(T), culture);

        public static T To<T>(this string s) => s.To<T>(CultureInfo.InvariantCulture);

        public static object To(this string s, Type desired) => s.To(desired, CultureInfo.InvariantCulture);

        public static object To(this string s, Type desired, CultureInfo culture)
        {
            if (typeof(string) == desired)
                return s;

            if (typeof(Enum).IsAssignableFrom(desired))
            {
                try
                {
                    return Enum.Parse(desired, s, true); // supports everything including parsing enums with flags starting with .net 4.0
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(string.Format("{0} cannot be converted to {1}", s, desired.FullName), e);
                }
            }

            if (desired.IsGenericType && typeof(Nullable<>) == desired.GetGenericTypeDefinition())
            {
                var underlyingType = Nullable.GetUnderlyingType(desired);

                if (String.IsNullOrEmpty(s))
                    return null;

                var value = s.To(underlyingType, culture);
                return Activator.CreateInstance(desired, value);
            }

            // bool has no .Parse:
            if (typeof(bool) == desired)
            {
                if (string.Equals(bool.TrueString, s, StringComparison.InvariantCultureIgnoreCase))
                    return true;

                if (string.Equals(bool.FalseString, s, StringComparison.InvariantCultureIgnoreCase))
                    return false;

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dec))
                    return Convert.ToBoolean(dec);

                throw new InvalidOperationException("The value does not represent a boolean: " + s);
            }

            if (string.IsNullOrEmpty(s))
                return desired.DefaultValue();

            // look if the type has the method Parse:
            MethodInfo parse = desired.GetMethod("Parse", new [] { typeof(string), typeof(IFormatProvider) });

            if (parse != null)
                return parse.Invoke(null, new object[] { s, culture });

            parse = desired.GetMethod("Parse", new[] { typeof(string) });
            if (parse != null)
                return parse.Invoke(null, new object[] { s });

            // look if there is a constructor accepting a string:
            var ctor = desired.GetConstructor(new[] { typeof(string) });
            if (ctor != null)
            {
                return Activator.CreateInstance(desired, s);
            }

            if (desired == typeof(object))
                return s;

            if (typeof(IConvertible).IsAssignableFrom(desired)) // Convert relies on IConvertible
            {
                // last desperate attempt:
                return Convert.ChangeType(s, desired, culture);
            }

            return desired.DefaultValue();
        }

        public static ConstantExpression ToExpression(this string s, Type desired) => Expression.Constant(s.To(desired));

        public static string CrLf(this string s) => s.CrLf(1);

        public static string CrLf(this string s, int count) => Enumerable
            .Repeat(Environment.NewLine, count)
            .Aggregate(new StringBuilder(s), (text, crlf) => text.Append(crlf)).ToString();

        public static string[] SplitBy(this string s, params char[] separators)
            => s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        public static string[] SplitBy(this string s, params string[] separators)
            => s.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        public static string Strip(this string s, string toStrip)
            => Regex.Replace(s, toStrip + "$", string.Empty);

        public static string CSharpify(this string identifier)
            => identifier.Replace(@"[\.]", match => ""); // todo: implement properly
    }
}